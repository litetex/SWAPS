using CommandLine;
using CoreFrameworkBase.Crash;
using CoreFrameworkBase.Logging;
using CoreFrameworkBase.Logging.Initalizer;
using CoreFrameworkBase.Logging.Initalizer.Impl;
using SWAPS.CMD;
using SWAPS.Shared;
using System;
using System.Collections.Generic;
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
         if(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
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
                     CurrentLoggerInitializer.InitializeWith(GetLoggerInitializer(opt.LogToFile));

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

      static ILoggerInitializer GetLoggerInitializer(bool writeFile = false) =>
         new SWAPSDefaultLoggerInitializer()
         {
            WriteFile = writeFile
         };
   }
}
