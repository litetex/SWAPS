using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SWAPS.AdminCom.Util;
using SWAPS.Shared.Com.IPC.Payload;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SWAPS.AdminCom.Service
{
   public class WSSyncServerRequester<S,R> : WebSocketBehavior 
   {
      public ServiceManager<S,R> ServiceManager { get; set; }

      public WSSyncServerRequester()
      {
         
      }

      protected override void OnMessage(MessageEventArgs e)
      {
         if(ServiceManager == null)
         {
            SWAPS.Log.Warn("Not initalized; Skipping onMessage");
            return;
         }

         ServiceManager.OnMessage(e);
      }

      protected override void OnError(ErrorEventArgs e)
      {
         if (ServiceManager == null)
         {
            SWAPS.Log.Warn("Not initalized; Skipping onError");
            return;
         }

         ServiceManager.OnError(e);
      }
   }
}
