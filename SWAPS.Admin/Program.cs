using CommandLine;
using CoreFrameworkBase.Crash;
using CoreFrameworkBase.Logging;
using CoreFrameworkBase.Logging.Initalizer;
using CoreFrameworkBase.Logging.Initalizer.Impl;
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
         if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
         {
            CurrentLoggerInitializer.InitializeWith(GetLoggerInitializer());
            Log.Error("Only Windows is supported");
            Environment.Exit(-1);
            return;
         }

#if !DEBUG
         try
         {

            new CrashDetector()
            {
               LoggerInitializer = GetLoggerInitializer(true)
            }.Init();
#endif
         Parser.Default.ParseArguments<CmdOption>(args)
                  .WithParsed((opt) =>
                  {
                     CurrentLoggerInitializer.InitializeWith(GetLoggerInitializer(opt.LogToFile, opt.LogFilePathBase64));

                     var starter = new StartUp(opt);
                     starter.Start();
                  })
                  .WithNotParsed((ex) =>
                  {
                     CurrentLoggerInitializer.InitializeWith(GetLoggerInitializer());
                     foreach (var error in ex)
                        Log.Error($"Failed to parse: {error.Tag}");
                  });
#if !DEBUG
         }
         catch (Exception ex)
         {

            CurrentLoggerInitializer.InitializeWith(GetLoggerInitializer(true));
            Log.Fatal(ex);

         }
#endif
      }

      static ILoggerInitializer GetLoggerInitializer(bool writeFile = false, string logfilePathBase64 = null)
      {
         string logfilePath =
            logfilePathBase64 == null ? null :
            Encoding.UTF8.GetString(Convert.FromBase64String(logfilePathBase64));

         return new SWAPSDefaultLoggerInitializer()
         {
            WriteFile = writeFile,
            PresetLogfilePath = logfilePath,
            OutputTemplateFile = "{Timestamp:HH:mm:ss,fff} {Log4NetLevel} [ADM] {ThreadId,-2} {Message:lj}{NewLine}{Exception}",
         };
      }
   }
}
