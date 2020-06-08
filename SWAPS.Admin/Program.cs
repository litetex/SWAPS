using CommandLine;
using CoreFramework.CrashLogging;
using CoreFramework.Logging;
using CoreFramework.Logging.Initalizer;
using CoreFramework.Logging.Initalizer.Impl;
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
            InitLog();
            Log.Error("Only Windows is supported");
            Environment.Exit(-1);
            return;
         }

#if !DEBUG
         try
         {

            new CrashDetector()
            {
               SupplyLoggerInitalizer = () => {
                  InitLog(li => li.Config.WriteFile = true);
                  return CurrentLoggerInitializer.Current;
               }
            }.Init();
#endif
         Parser.Default.ParseArguments<CmdOption>(args)
                  .WithParsed((opt) =>
                  {
                     InitLog(li => {
                        li.Config.WriteFile = opt.LogToFile;
                        li.PresetLogfilePath = DecodeLogFilePathFromBase64(opt.LogFilePathBase64);
                     });

                     var starter = new StartUp(opt);
                     starter.Start();
                  })
                  .WithNotParsed((ex) =>
                  {
                     InitLog();
                     foreach (var error in ex)
                        Log.Error($"Failed to parse: {error.Tag}");
                  });
#if !DEBUG
         }
         catch (Exception ex)
         {

            InitLog(li => li.Config.WriteFile = true);
            Log.Fatal(ex);

         }
#endif
      }

      static void InitLog(Action<SWAPSDefaultLoggerInitializer> initAction = null)
      {
         CurrentLoggerInitializer.InitLogging(il =>
         {
            initAction?.Invoke((SWAPSDefaultLoggerInitializer)il);
            ((SWAPSDefaultLoggerInitializer)il).Config.OutputTemplateFile = "{Timestamp:HH:mm:ss,fff} {Log4NetLevel} [ADM] {ThreadId,-2} {Message:lj}{NewLine}{Exception}";
         });
      }

      static string DecodeLogFilePathFromBase64(string logfilePathBase64)
      {
         return logfilePathBase64 == null ? null :
            Encoding.UTF8.GetString(Convert.FromBase64String(logfilePathBase64));
      }
   }
}
