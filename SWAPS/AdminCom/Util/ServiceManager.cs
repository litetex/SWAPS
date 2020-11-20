using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SWAPS.Shared.Com.IPC.Payload;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SWAPS.AdminCom.Util
{
   public class ServiceManager<S, R>
   {
      protected Dictionary<Guid, TaskCompletionSource<R>> WaitingTasks { get; set; } = new Dictionary<Guid, TaskCompletionSource<R>>();

      public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

      public Action<string> Broadcaster { get; set; }

      protected CancellationToken CancelOperationToken { get; private set; }

      public ServiceManager(Func<CancellationToken> tokenProvider)
      {
         CancelOperationToken = tokenProvider();
      }

      public void OnMessage(MessageEventArgs e)
      {
         var parsedData = Encoding.UTF8.GetString(Convert.FromBase64String(e.Data));

         Log.Debug("onMessage", parsedData);

         var answer = JsonConvert.DeserializeObject<ComAnswerWrapper<R>>(parsedData);

         WaitingTasks[answer.OriginalID].TrySetResult(answer.PayLoad);
      }

      public void OnError(ErrorEventArgs e)
      {
         Log.Debug("onError", e.Exception);

         if (!(e.Exception is IComException ex))
         {
            Log.Warn($"onError: Transmitted error is not {nameof(IComException)}", e.Exception);
            return;
         }

         WaitingTasks[ex.OriginalID].TrySetException(e.Exception);
      }

      public async Task<R> Invoke(S msgToSend, TimeSpan? timeout = null)
      {
         var msgID = Guid.NewGuid();

         var msg = JsonConvert.SerializeObject(new ComMessageWrapper<S>()
         {
            PayLoad = msgToSend,
            ID = msgID
         });

         var tcs = new TaskCompletionSource<R>();
         WaitingTasks.Add(msgID, tcs);
         try
         {
            var encMsg = Convert.ToBase64String(Encoding.UTF8.GetBytes(msg));
            Log.Debug($"Broadcasting message: {encMsg}");
            Broadcaster(encMsg);

            var useTimeout = timeout ?? Timeout;

            using var timeoutCancellationTokenSource = new CancellationTokenSource();
            using var ctReg = CancelOperationToken.Register(timeoutCancellationTokenSource.Cancel);

            if(await Task.WhenAny(Task.Delay(useTimeout, timeoutCancellationTokenSource.Token), tcs.Task) == tcs.Task)
            {
               timeoutCancellationTokenSource.Cancel();
               return tcs.Task.Result;
            }
            else
               throw new TimeoutException("Task timed out");
         }
         finally
         {
            WaitingTasks.Remove(msgID);
         }
      }

   }
}
