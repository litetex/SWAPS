using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SWAPS.AdminCom.Util;
using SWAPS.Shared.Com.IPC.Payload;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SWAPS.AdminCom.Service
{
   public class WSSyncServerRequester<S,R> : WebSocketBehavior 
   {
      protected ServiceManager<S,R> ServiceManager { get; set; }

      public WSSyncServerRequester(ServiceManager<S, R> serviceManager)
      {
         ServiceManager = serviceManager;
      }

      protected override void OnMessage(MessageEventArgs e)
      {
         ServiceManager.OnMessage(e);
      }

      protected override void OnError(ErrorEventArgs e)
      {
         ServiceManager.OnError(e);
      }
   }
}
