using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SWAPS.AdminCom.Service
{
   public class WSReflector : WebSocketBehavior
   {
      protected override void OnMessage(MessageEventArgs e)
      {
         Send(e.Data);
      }
   }
}
