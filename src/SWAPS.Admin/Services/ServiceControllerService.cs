using SWAPS.Admin.Service;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace SWAPS.Admin.Services
{
   /// <summary>
   /// Controlls Windows Services
   /// </summary>
   public class ServiceControllerService
   {
      private void TryChangeServiceStartUpType(ServiceController svc, ServiceStartMode startMode)
      {
         try
         {
            if (svc.StartType != startMode)
            {
               Log.Info($"Changing '{svc.ServiceName}' StartUpType to '{startMode}'");
               ServiceHelper.ChangeStartMode(svc, startMode);
            }
         }
         catch (Exception e)
         {
            Log.Error($"Failed to change '{svc.ServiceName}'-{nameof(svc.StartType)} to {nameof(ServiceStartMode.Manual)}", e);
         }
      }

      private ServiceController GetService(string name)
      {
         return new ServiceController(name);
      }

      private void StartService(ServiceController svc)
      {
         Log.Info($"Starting '{svc.ServiceName}'");
         svc.Start();
      }

      private void StopService(ServiceController svc)
      {
         if (svc.Status == ServiceControllerStatus.Stopped)
         {
            Log.Debug($"Service '{svc.ServiceName}' is already stopped");
            return;
         }
         
         Log.Info($"Stopping Service '{svc.ServiceName}'");
         svc.Stop();   
      }

      private void StopServiceAndServiceNotFoundCheck(ServiceController svc, bool crashOnServiceNotFound)
      {
         try
         {
            StopService(svc);
            Log.Info("Stopped");
         }
         catch (InvalidOperationException invOpex)
         {
            Log.Error($"{nameof(InvalidOperationException)} while shuting down service");
            if (invOpex.InnerException is Win32Exception win32ex)
            {
               Log.Error("Win32Error: Code=" + win32ex.NativeErrorCode);
               if (win32ex.NativeErrorCode != 1060)
                  throw;

               Log.Warn("Program maybe updating: Service couldn't be found");
               if (crashOnServiceNotFound)
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

      public void StartUp(string name, TimeSpan serviceStartTimeout)
      {
         using var svc = GetService(name);

         StopService(svc);
         TryChangeServiceStartUpType(svc, ServiceStartMode.Manual);
         StartService(svc);
         WaitForServiceReachingStatus(svc, ServiceControllerStatus.Running, serviceStartTimeout);
      }

      public void ShutDown(string name, bool crashOnServiceNotFound)
      {
         using var svc = GetService(name);

         StopServiceAndServiceNotFoundCheck(svc, crashOnServiceNotFound);
      }
   }
}
