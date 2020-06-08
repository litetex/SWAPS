using CoreFramework.Logging;
using SWAPS.AdminCom;
using SWAPS.AdminCom.Service;
using SWAPS.Config;
using SWAPS.Shared.Admin.Services;
using SWAPS.Service;
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

      public Runner(Configuration configuration)
      {
         Config = configuration;
      }

      public void Run()
      {
         try
         {
            Console.Title = Config.Name;

            AdminCommunictator = new AdminCommunictator();
            AdminCommunictator.Start();

            using (var serviceCom = new ServiceCom<IServiceControllerService>(AdminCommunictator.PublicClient, TimeSpan.FromSeconds(10)))
            {
               serviceCom.RPC(s => s.StartUp(Config.ServiceConfig.ServiceName, Config.ServiceStartTimeout));

               Log.Info($"Waiting {Config.ServiceProperlyStartedDelay} for the service to become fully operational");
               Thread.Sleep((int)Config.ServiceProperlyStartedDelay.TotalMilliseconds);

               StartProgramAndWait();

               serviceCom.RPC(s => s.ShutDown(Config.ServiceConfig.ServiceName, Config.CrashOnUpdateServiceNotFound));
            }

            Thread.Sleep((int)Config.StayingOpenBeforeEnding.TotalMilliseconds);
         }
         catch (Exception e)
         {
            Log.Error(e);
            Console.ReadKey();
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
