using System;
using System.Collections.Generic;
using System.Text;
using SWAPS.Shared.Com.IPC.Payload;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace SWAPS.AdminCom.Service
{
   public class WSServiceStart : WebSocketBehavior
   {
      /* TODO: Create "wrapper" that contains
       * A blocking method (StartService(string name, TimeSpan timeout)
       *   -> Broadcasts (IMessage & IServiceStart)
       *   -> Waits for feedback from OnMessage (TaskCompletionSource / await / Map<Guid, TaskComplet...>?)
       *   // https://stackoverflow.com/a/35158815
       *   
       * Trigger an event here when onMessage is fired (see below)
       */

      protected override void OnMessage(MessageEventArgs e)
      {
         // emit event with (IAnswer & IServiceStart)  
      }
   }
}
