using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SWAPS.AdminCom.Service
{
   public class WsHandshake : WebSocketBehavior
   {
      private HandshakeWithinTimeout HandshakeWithinTimeout { get; set; }

      public WsHandshake(HandshakeWithinTimeout handshakeWithinTimeout)
      {
         HandshakeWithinTimeout = handshakeWithinTimeout;
      }

      protected override void OnMessage(MessageEventArgs e)
      {
         HandshakeWithinTimeout.Handshake();
      }
   }
}
