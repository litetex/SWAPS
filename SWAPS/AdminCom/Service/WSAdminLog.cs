﻿using System;
using System.Collections.Generic;
using System.Text;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SWAPS.AdminCom.Service
{
   public class WSAdminLog : WebSocketBehavior
   {
      protected override void OnMessage(MessageEventArgs e)
      {
         SWAPS.Log.Info($"ADMIN >> {e.Data}");
      }
   }
}
