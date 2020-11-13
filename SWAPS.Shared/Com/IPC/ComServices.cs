using System;
using System.Collections.Generic;
using System.Text;
using SWAPS.Shared.Com.IPC.Payload;

namespace SWAPS.Shared.Com.IPC
{
   public static class ComServices
   {
      /// <summary>
      /// Reflects all data back to the sender
      /// </summary>
      public const string REFLECTOR = "/reflector";

      /// <summary>
      /// Handshake
      /// </summary>
      public const string HANDSHAKE = "/handshake";


      /// <summary>
      /// Starts a service, see <see cref="IServiceStart"/>
      /// </summary>
      public const string SERVICE_START = "/service/start";

      /// <summary>
      /// Stops a service, see <see cref="IServiceStart"/>
      /// </summary>
      public const string SERVICE_STOP = "/service/stop";


      /// <summary>
      /// Any data send by this channels tells the admin-process to shutdown
      /// </summary>
      public const string SHUTDOWN_ADMIN = "/shutdown/admin";
   }
}
