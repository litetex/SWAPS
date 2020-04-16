using JKang.IpcServiceFramework;
using SWAPS.Shared.Admin;
using SWAPS.Shared.Admin.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SWAPS.AdminCom.Service
{
   public class ServiceCom<T> : IDisposable where T : class
   {
      private IpcServiceClient<IAdminControllerService> AdminClient { get; set; }

      private int ComPort { get; set; }

      private IpcServiceClient<T> Client { get; set; }

      private TimeSpan Timeout { get; set; }

      public ServiceCom(IpcServiceClient<IAdminControllerService> adminClient, TimeSpan timeout)
      {
         AdminClient = adminClient;
         ComPort = AdminClient.InvokeAsync(s => s.StartServiceInstance<T>()).Result;

         Client = new IpcServiceClientBuilder<T>()
            .UseTcp(IPAddress.Loopback, ComPort)
            .Build();

         Timeout = timeout;
      }

      public void RPC(Action<T> call, TimeSpan? timeout = null)
      {
         TimeSpan timeOut = timeout ?? Timeout;

         using var ts = new CancellationTokenSource();

         if (!Client.InvokeAsync(s => call.Invoke(s), ts.Token).Wait(timeOut))
         {
            ts.Cancel();
            throw new TimeoutException($"RPC call timed out [{timeOut}]");
         }
      }

      public R RPC<R>(Func<T, R> call, TimeSpan? timeout = null)
      {
         TimeSpan timeOut = timeout ?? Timeout;

         using var ts = new CancellationTokenSource();

         Task<R> task = Client.InvokeAsync(s => call.Invoke(s));
         if(!task.Wait(timeOut))
         {
            ts.Cancel();
            throw new TimeoutException($"RPC call timed out [{timeOut}]");
         }

         return task.Result;
      }

      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      /// <param name="disposing">true when call from Dispose(), false when call from finalizer / deconstructor</param>
      protected virtual void Dispose(bool disposing)
      {
         if (!disposing)
            return;

         Log.Debug($"Sending request to stop Service on RemotePort {ComPort}");
         AdminClient.InvokeAsync(s => s.StopServiceOnPort(ComPort));
      }
   }
}
