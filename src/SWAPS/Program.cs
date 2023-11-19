using CommandLine;
using Serilog;
using SWAPS.CMD;
using SWAPS.Shared;
using SWAPS.StartUp;
using SWAPS.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SWAPS
{
   /// <summary>
   /// Main entry point of SWAPS - start programm without auto-starting service
   /// </summary>
   public static class Program
   {
      static void Main(string[] args)
      {
         Run(args);
      }

      public static void Run(string[] args)
      {
         Serilog.Log.Logger = GetDefaultLoggerConfiguration().CreateLogger();

         AppDomain.CurrentDomain.ProcessExit += (s, ev) =>
         {
            Log.Debug("Shutting down logger; Flushing...");
            Serilog.Log.CloseAndFlush();
         };

         if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
         {
            Log.Error("Only Windows is supported");
            Environment.Exit(-1);
            return;
         }

#if !DEBUG
         try
         {
            AppDomain.CurrentDomain.UnhandledException += (s, ev) =>
            {
               try
               {
                  ConsoleController.TryShowConsole();

                  if (ev?.ExceptionObject is Exception ex)
                  {
                     Log.Fatal("An unhandled error occured", ex);
                     return;
                  }

                  Log.Fatal($"An unhandled error occured {ev}");
                  
               }
               catch (Exception ex)
               {
                  Console.Error.WriteLine($"Failed to catch unhandled error '{ev?.ExceptionObject ?? ev}': {ex}");
               }
            };
#endif
            GetDefaultParser()
               .ParseArguments<RunCmdOptions, GenConfigOptions, UpdateCmdOptions>(args)
               .WithParsed<UpdateCmdOptions>(opt => Exec(opt, o => new UpdateStartUp(o)))
               .WithParsed<GenConfigOptions>(opt => Exec(opt, o => new GenConfigStartUp(o)))
               .WithParsed<RunCmdOptions>(opt => Exec(opt, o => new RunStartUp(o)))
               .WithNotParsed(ParserFail);
#if !DEBUG
         }
         catch (Exception ex)
         {
            ConsoleController.TryShowConsole();
            Log.Fatal(ex);
         }
#endif
      }
      private static Parser GetDefaultParser()
      {
         return new Parser(settings =>
         {
            settings.IgnoreUnknownArguments = true;
            settings.CaseSensitive = false;
            settings.CaseInsensitiveEnumValues = true;
         });
      }

      private static void Exec<O,S>(O opts, Func<O, S> startUpSupplierFunc) 
         where O : AbstractCmdOptions
         where S : AbstractStartUp<O>
      {
         ExecuteDefault(opts);

         startUpSupplierFunc.Invoke(opts).Run();
      }

      private static void ExecuteDefault(AbstractCmdOptions standardOpt)
      {
         var logConf = GetDefaultLoggerConfiguration();
         if (standardOpt.Verbose)
         {
            logConf.MinimumLevel.Debug();

            Log.Info("Running in verbose mode");
         }
         if (standardOpt.LogToFile)
         {
            logConf.WriteTo.File(Path.Combine("logs", "log.log"),
                  outputTemplate: "{Timestamp:HH:mm:ss,fff} {Level:u3} {ThreadId,-2} {Message:lj}{NewLine}{Exception}",
                  rollingInterval: RollingInterval.Day,
                  rollOnFileSizeLimit: true,
                  retainedFileCountLimit: standardOpt.LogFileRetainCount >= 0 ? standardOpt.LogFileRetainCount : (int?)null);

            Log.Info("Logger will also write to file");
         }
         Serilog.Log.Logger = logConf.CreateLogger();
      }

      private static void ParserFail(IEnumerable<Error> errors)
      {
         if (errors.All(err =>
                           new ErrorType[]
                           {
                           ErrorType.HelpRequestedError,
                           ErrorType.HelpVerbRequestedError,
                           ErrorType.UnknownOptionError
                           }.Contains(err.Tag))
                     )
            return;

         foreach (var error in errors)
            Log.Error($"Failed to parse: {error.Tag}");

         Log.Fatal("Failed to process args");
      }

      private static LoggerConfiguration GetDefaultLoggerConfiguration()
      {
         return new LoggerConfiguration()
            .Enrich.WithThreadId()
            .MinimumLevel
#if DEBUG
               .Debug()
#else
               .Information()
#endif
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss,fff} {Level:u3} {ThreadId,-2} {Message:lj}{NewLine}{Exception}");
      }
   }
}
