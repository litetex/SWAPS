using CommandLine;
using CoreFrameworkBase.Crash;
using CoreFrameworkBase.Logging;
using CoreFrameworkBase.Logging.Initalizer;
using CoreFrameworkBase.Logging.Initalizer.Impl;
using SWAPS.CMD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SWAPS
{
   /// <summary>
   /// Main entry point of SWAPS - start programm without auto-starting service
   /// </summary>
   /// 
   public static class Program
   {
      static void Main(string[] args)
      {
         Run(args);
      }

      public static void Run(string[] args)
      {
         CurrentLoggerInitializer.Set(new DefaultLoggerInitializer(new DefaultLoggerInitializerConfig()));

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
                     InitLog(li => li.Config.WriteFile = opt.LogToFile);

                     var starter = new StartUp(opt);
                     starter.Start();
                  })
                  .WithNotParsed((ex) =>
                  {
                     if (ex.All(err =>
                             new ErrorType[]
                             {
                                 ErrorType.HelpRequestedError,
                                 ErrorType.HelpVerbRequestedError
                             }.Contains(err.Tag))
                       )
                        return;

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

      static void InitLog(Action<DefaultLoggerInitializer> initAction = null)
      {
         CurrentLoggerInitializer.InitLogging(il => initAction?.Invoke((DefaultLoggerInitializer)il));
      }
   }
}
