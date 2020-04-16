using CommandLine;
using CoreFrameworkBase.Crash;
using CoreFrameworkBase.Logging;
using SWAPS.Admin.CMD;
using System;
using System.Collections.Generic;
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
         if(!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
         {
            LoggerInitializer.Current.InitLogger(false);
            Log.Error("Only Windows is supported");
            Environment.Exit(-1);
            return;
         }

#if !DEBUG
         try
         {

            CrashDetector.Current.Init();
#endif
         Parser.Default.ParseArguments<CmdOption>(args)
                  .WithParsed((opt) =>
                  {
                     LoggerInitializer.Current.InitLogger(opt.LogToFile);

                     var starter = new StartUp(opt);
                     starter.Start();
                  })
                  .WithNotParsed((ex) =>
                  {
                     LoggerInitializer.Current.InitLogger();
                     foreach (var error in ex)
                        Log.Error($"Failed to parse: {error.Tag}");
                  });
#if !DEBUG
         }
         catch (Exception ex)
         {

            LoggerInitializer.Current.InitLogger(true);
            Log.Fatal(ex);

         }
#endif
      }
   }
}
