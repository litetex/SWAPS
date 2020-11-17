using CommandLine;
using Serilog;
using SWAPS.Admin.CMD;
using SWAPS.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SWAPS.Admin
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

         try
         {
            AppDomain.CurrentDomain.UnhandledException += (s, ev) =>
            {
               try
               {
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

            var parser = new Parser(settings =>
            {
               settings.CaseSensitive = false;
            });
            parser.ParseArguments<CmdOptions>(args)
                     .WithParsed((opt) =>
                     {
                        if (opt.LogToFile)
                        {
                           var logConf = GetDefaultLoggerConfiguration();

                           logConf.WriteTo.File(Path.Combine("logs", "adminlog.log"),
                                 outputTemplate: "{Timestamp:HH:mm:ss,fff} {Level:u3} {ThreadId,-2} {Message:lj}{NewLine}{Exception}",
                                 rollingInterval: RollingInterval.Day,
                                 rollOnFileSizeLimit: true);

                           Serilog.Log.Logger = logConf.CreateLogger();
                           Log.Info("Logger will also write to file");
                        }

                        var starter = new StartUp(opt);
                        starter.Start();
                     })
                     .WithNotParsed((ex) =>
                     {
                        foreach (var error in ex)
                           Log.Error($"Failed to parse: {error.Tag}");
                     });
         }
         catch (Exception ex)
         {
            Log.Fatal(ex);
         }
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
