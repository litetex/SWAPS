using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SWAPS
{
   public class HandshakeWithinTimeout
   {
      private TaskCompletionSource<bool> TCS { get; set; } = new TaskCompletionSource<bool>();

      public async Task<bool> StartTimeout(TimeSpan timeout, Action onTimeout = null)
      {
         using var timeoutCancellationTokenSource = new CancellationTokenSource();
         if (await Task.WhenAny(Task.Delay(timeout, timeoutCancellationTokenSource.Token), TCS.Task) == TCS.Task)
         {
            timeoutCancellationTokenSource.Cancel();
            await TCS.Task;
            return true;
         }
         else
         {
            onTimeout?.Invoke();
            return false;
         }

      }

      public void Handshake()
      {
         TCS.TrySetResult(true);
      }
   }
}
