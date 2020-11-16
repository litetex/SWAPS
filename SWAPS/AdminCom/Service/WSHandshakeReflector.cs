using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SWAPS.AdminCom.Service
{
   public class WSHandshakeReflector : WebSocketBehavior
   {
      private HandshakeWithinTimeout HandshakeWithinTimeout { get; set; }

      public WSHandshakeReflector(HandshakeWithinTimeout handshakeWithinTimeout)
      {
         HandshakeWithinTimeout = handshakeWithinTimeout;
      }

      protected override void OnMessage(MessageEventArgs e)
      {
         HandshakeWithinTimeout.Handshake();
      }
   }
}
