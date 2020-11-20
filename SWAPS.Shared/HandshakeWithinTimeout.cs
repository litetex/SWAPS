using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SWAPS
{
   public class HandshakeWithinTimeout : IDisposable
   {
      private bool disposedValue;

      private CancellationTokenSource AbortTS { get; set; } = new CancellationTokenSource();

      private TaskCompletionSource<bool> TCS { get; set; } = new TaskCompletionSource<bool>();

      public async Task<bool> StartTimeout(TimeSpan timeout)
      {
         using var timeoutCancellationTokenSource = new CancellationTokenSource();
         using var ctReg = AbortTS.Token.Register(timeoutCancellationTokenSource.Cancel);

         var timeoutTask = Task.Delay(timeout, timeoutCancellationTokenSource.Token);

         if (await Task.WhenAny(timeoutTask, TCS.Task) == TCS.Task)
         {
            timeoutCancellationTokenSource.Cancel();
            await TCS.Task;
            return true;
         }
         else
         {
            return false;
         }

      }

      public void Handshake()
      {
         TCS.TrySetResult(true);
      }

      public void Abort()
      {
         AbortTS.Cancel();
      }

      protected virtual void Dispose(bool disposing)
      {
         if (!disposedValue)
         {
            if (disposing)
            {
               Abort();
               AbortTS.Dispose();
            }

            disposedValue = true;
         }
      }

      public void Dispose()
      {
         // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
         Dispose(disposing: true);
         GC.SuppressFinalize(this);
      }
   }
}
