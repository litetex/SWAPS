using CoreFrameworkBase.Logging;
using SWAPS.Config;
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

            using (ServiceController svc = GetService())
            {
               CheckAndChangeServiceStartUpType(svc);
               StartService(svc);
               WaitForServiceReachingStatus(svc, ServiceControllerStatus.Running, Config.ServiceStartTimeout);
            }
            StartProgramAndWait();

            using (ServiceController svc = GetService())
               StopService(svc);

            Thread.Sleep((int)Config.StayingOpenBeforeEnding.TotalMilliseconds);
         }
         catch (Exception e)
         {
            Log.Error(e);
            Console.ReadKey();
         }

      }

      private void StopService(ServiceController svc)
      {
         Log.Info($"Stopping '{Config.ServiceConfig.ServiceName}'... Waiting 1 Sec");
         Thread.Sleep((int)Config.ServiceShutdownDelay.TotalMilliseconds);

         try
         {
            svc.Stop();
            Log.Info("Stopped");
         }
         catch (InvalidOperationException invOpex)
         {
            Log.Error($"{nameof(InvalidOperationException)} while shuting down service");
            if (invOpex.InnerException is Win32Exception win32ex)
            {
               Log.Error("Win32Error: Code=" + win32ex.ErrorCode);
               if (win32ex.ErrorCode != 1060)
                  throw;

               Log.Warn("Program maybe updating: Service couldn't be found");
               if (Config.CrashOnUpdateServiceNotFound)
                  throw;
            }
            else
               throw;
         }
         catch (Exception e)
         {
            Log.Error("Failed to stop service! ExceptionType: " + e.GetType());

            throw;
         }

      }

      private void StartService(ServiceController svc)
      {
         Log.Info($"Starting '{Config.ServiceConfig.ServiceName}'");
         svc.Start();
      }

      private void WaitForServiceReachingStatus(ServiceController svc, ServiceControllerStatus target, TimeSpan timeout)
      {
         var sw = Stopwatch.StartNew();

         var task = Task.Run(() =>
         {
            while (svc.Status != target)
            {
               Thread.Sleep(500);
               svc.Refresh();
            }
         });
         if (task.Wait(timeout))
         {
            sw.Stop();
            Log.Info($"{svc.DisplayName} reached status='{target}' in {sw.Elapsed}");
            return;
         }
         sw.Stop();

         throw new System.TimeoutException($"{svc.DisplayName} failed to reach status='{target}' in {timeout}!");
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
         else if(!process.WaitForExit((int)Config.ProcessConfig.Timeout.Value.TotalMilliseconds))
         {
            Log.Info("Process timed out, killing it");
            process.Kill();
         }
      }

      private void CheckAndChangeServiceStartUpType(ServiceController svc)
      {
         try
         {
            if (svc.Status != ServiceControllerStatus.Stopped)
            {
               Log.Info($"Stopping Service '{Config.ServiceConfig.ServiceName}'...");
               svc.Stop();
            }
            if (svc.StartType != ServiceStartMode.Manual)
            {
               Log.Info($"Changing '{Config.ServiceConfig.ServiceName}' StartUpType to Manual");
               ServiceHelper.ChangeStartMode(svc, ServiceStartMode.Manual);
            }

         }
         catch (Exception e)
         {
            Log.Error($"Failed to change '{Config.ServiceConfig.ServiceName}'-{nameof(svc.StartType)} to {nameof(ServiceStartMode.Manual)}", e);
         }
      }


      private ServiceController GetService()
      {
         return new ServiceController(Config.ServiceConfig.ServiceName);
      }

   }
}
