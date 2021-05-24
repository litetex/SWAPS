using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SWAPS.AdminCom.Service
{
   public class WSHandshakeReflector : WebSocketBehavior
   {
      public HandshakeWithinTimeout HandshakeWithinTimeout { get; set; }

      protected override void OnMessage(MessageEventArgs e)
      {
         if(HandshakeWithinTimeout == null)
         {
            SWAPS.Log.Warn("Not initalized; Skipping onMessage");
            return;
         }

         SWAPS.Log.Debug($"onMessage: {e.Data}");
         HandshakeWithinTimeout.Handshake();
         Send(e.Data);
      }
   }
}
