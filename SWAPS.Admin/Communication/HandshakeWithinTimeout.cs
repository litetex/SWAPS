using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SWAPS.Admin.Communication
{
   public class HandshakeWithinTimeout
   {
      private TaskCompletionSource<bool> TCS { get; set; } = new TaskCompletionSource<bool>();

      public void StartTimeout(TimeSpan timeout, Action onTimeout)
      {
         Task.Run(() =>
         {
            Task.WaitAny(Task.Delay((int)timeout.TotalMilliseconds), TCS.Task);

            if (!TCS.Task.IsCompleted)
               onTimeout();
         });
      }

      public void Handshake()
      {
         TCS.TrySetResult(true);
      }
   }
}
