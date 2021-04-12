using System;
using System.Collections.Generic;
using System.Text;
using SWAPS.AdminCom.Util;
using SWAPS.Shared.Com.IPC.Payload;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SWAPS.AdminCom.Service
{
   public class WSServiceStart : WSSyncServerRequester<ServiceStart,bool>
   {
      // Just generics
   }
}
