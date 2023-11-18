using CommandLine;
using Serilog;
using SWAPS.Admin.CMD;
using SWAPS.Admin.Logging;
using SWAPS.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SWAPS.Admin
{
   /// <summary>
   /// Main entry point of SWAPS.Admin - start programm without auto-starting service (admin process)
   /// </summary>
   public static class Program
   {
      public static Action<string> Writer { get; set; } = s => { };
      public static Func<bool> WriterAvailable { get; set; } = () => false;

      // Fix for https://github.com/commandlineparser/commandline/issues/848
      [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(CmdOptions))]
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
            InstallUnhandledExceptionHandler();

            var parser = new Parser(settings =>
            {
               settings.CaseSensitive = false;
            });
            parser.ParseArguments<CmdOptions>(args)
                     .WithParsed((opt) =>
                     {
                        if(!opt.ShowServerConsole)
                           ShowWindow(GetConsoleWindow(), SW_HIDE);

                        var logConf = GetDefaultLoggerConfiguration();
                        if (opt.Verbose)
                        {
                           logConf.MinimumLevel.Debug();

                           Serilog.Log.Logger = logConf.CreateLogger();
                           Log.Info("Running in verbose mode");
                        }
                        if (opt.LogToFile)
                        {
                           logConf.WriteTo.File(Path.Combine("logs", "adminlog.log"),
                                 outputTemplate: "{Timestamp:HH:mm:ss,fff} {Level:u3} {ThreadId,-2} {Message:lj}{NewLine}{Exception}",
                                 rollingInterval: RollingInterval.Day,
                                 rollOnFileSizeLimit: true,
                                 retainedFileCountLimit: opt.LogFileRetainCount >= 0 ? opt.LogFileRetainCount : (int?)null);

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
            .WriteTo.Console(outputTemplate: "{Timestamp:HH:mm:ss,fff} {Level:u3} {ThreadId,-2} {Message:lj}{NewLine}{Exception}")
            .WriteTo.Buffered(s => Writer(s), () => WriterAvailable(), outputTemplate: "{Timestamp:HH:mm:ss,fff} {Level:u3} {ThreadId,-2} {Message:lj}{NewLine}{Exception}");
      }

      private static void InstallUnhandledExceptionHandler()
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
      }

      [DllImport("kernel32.dll")]
      static extern IntPtr GetConsoleWindow();

      [DllImport("user32.dll")]
      static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

      // Hide is 0; Show is 5
      const int SW_HIDE = 0;
   }
}
