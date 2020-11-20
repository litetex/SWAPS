using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using SWAPS.Admin.Communication;
using SWAPS.Shared.Com.IPC;
using SWAPS.Shared.Com.IPC.Payload;

namespace SWAPS.Admin.Services
{
   public class ServiceLinker
   {
      private Communicator Communicator { get; set; }

      public ServiceLinker(Communicator communicator)
      {
         Communicator = communicator;
      }

      public void LinkServices()
      {
         Log.Info("Linking services to endpoints");
         var scs = new ServiceControllerService();

         var wsStartWebSocket = Communicator.CreateWebSocket(ComServices.S_SERVICE_START);
         wsStartWebSocket.OnOpen += (s, ev) => 
         {
            Log.Debug($"{ComServices.S_SERVICE_START} onOpen");
         };
         wsStartWebSocket.OnMessage += (s, ev) =>
         {
            Log.Debug($"{ComServices.S_SERVICE_START} got message: {ev.Data}");
            wsStartWebSocket.Send(ProcessMessageAndAnswer<ServiceStart,bool>(ev.Data, ss => 
            {
               scs.StartUp(ss.Name, ss.Timeout);
               return true;
            }));
         };
         wsStartWebSocket.Connect();
         Log.Info($"Linked {ComServices.S_SERVICE_START}");

         var wsStopWebSocket = Communicator.CreateWebSocket(ComServices.S_SERVICE_STOP);
         wsStartWebSocket.OnOpen += (s, ev) =>
         {
            Log.Debug($"{ComServices.S_SERVICE_START} onOpen");
         };
         wsStopWebSocket.OnMessage += (s, ev) =>
         {
            Log.Debug($"{ComServices.S_SERVICE_STOP} got message: {ev.Data}");
            wsStopWebSocket.Send(ProcessMessageAndAnswer<ServiceStop, bool>(ev.Data, ss =>
            {
               scs.ShutDown(ss.Name, ss.CrashOnServiceNotFound);
               return true;
            }));
         };
         wsStopWebSocket.Connect();
         Log.Info($"Linked {ComServices.S_SERVICE_STOP}");
      }

      private string ProcessMessageAndAnswer<M,A>(string msgBase64, Func<M,A> processor)
      {
         var decodedMessage = JsonConvert.DeserializeObject<ComMessageWrapper<M>>(Encoding.UTF8.GetString(Convert.FromBase64String(msgBase64)));

         var processoResult = processor.Invoke(decodedMessage.PayLoad);

         var answer = JsonConvert.SerializeObject(new ComAnswerWrapper<A>()
         {
            ID = Guid.NewGuid(),
            OriginalID = decodedMessage.ID,
            PayLoad = processoResult
         });

         return Convert.ToBase64String(Encoding.UTF8.GetBytes(answer));
      }
   }
}
