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
            Broadcaster(Convert.ToBase64String(Encoding.UTF8.GetBytes(msg)));

            var useTimeout = timeout ?? Timeout;

            await Task.WhenAny(Task.Delay((int)useTimeout.TotalMilliseconds), tcs.Task);

            if (!tcs.Task.IsCompleted)
               throw new TimeoutException("Task timed out");

            return tcs.Task.Result;
         }
         finally
         {
            WaitingTasks.Remove(msgID);
         }
      }
   }
}
