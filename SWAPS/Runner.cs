using SWAPS.AdminCom;
using SWAPS.AdminCom.Service;
using SWAPS.CMD;
using SWAPS.Config;
using SWAPS.Shared.Com.IPC.Payload;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace SWAPS
{
   public class Runner
   {
      protected AdminCommunictator AdminCommunictator { get; set; }

      protected Configuration Config { get; set; }

      protected CmdOptions CmdOptions { get; set; }

      public Runner(Configuration configuration, CmdOptions cmdOptions)
      {
         Config = configuration;
         CmdOptions = cmdOptions;
      }

      public void Run()
      {
         try
         {
            Console.Title = Config.Name;

            AdminCommunictator = new AdminCommunictator(CmdOptions.LogToFile);
            AdminCommunictator.Start();

            AdminCommunictator.StartServiceManager
               .Invoke(
               new ServiceStart()
               {
                  Name = Config.ServiceConfig.ServiceName,
                  Timeout = Config.ServiceStartTimeout
               },
               Config.ServiceStartTimeout.Add(TimeSpan.FromSeconds(10)))
               .Wait();

            Log.Info($"Waiting {Config.ServiceProperlyStartedDelay} for the service to become fully operational");
            Thread.Sleep((int)Config.ServiceProperlyStartedDelay.TotalMilliseconds);

            StartProgramAndWait();

            AdminCommunictator.StopServiceManager
               .Invoke(
               new ServiceStop()
               {
                  Name = Config.ServiceConfig.ServiceName,
                  CrashOnServiceNotFound = Config.CrashOnUpdateServiceNotFound
               },
               TimeSpan.FromSeconds(10))
               .Wait();

            Thread.Sleep((int)Config.StayingOpenBeforeEnding.TotalMilliseconds);
         }
         catch (Exception e)
         {
            Log.Error(e);
         }
         finally
         {
            AdminCommunictator.Stop();
         }

      }

      private void StartProgramAndWait()
      {
         Log.Info($"Starting '{Config.ProcessConfig.FilePath}' in '{Config.ProcessConfig.WorkDir}'...");
         Process process = new Process
         {
            StartInfo = new ProcessStartInfo
            {
               WorkingDirectory = Config.ProcessConfig.WorkDir,
               FileName = Config.ProcessConfig.FilePath,
               Arguments = Config.ProcessConfig.Args,
            }
         };

         process.Start();

         if (Config.ProcessConfig.Timeout == null)
            process.WaitForExit();
         else if (!process.WaitForExit((int)Config.ProcessConfig.Timeout.Value.TotalMilliseconds))
         {
            Log.Info("Process timed out, killing it");
            process.Kill();
         }
      }

   }
}
