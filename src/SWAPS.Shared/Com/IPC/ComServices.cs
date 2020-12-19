using System;
using System.Collections.Generic;
using System.Text;
using SWAPS.Shared.Com.IPC.Payload;

namespace SWAPS.Shared.Com.IPC
{
   public static class ComServices
   {
      /// <summary>
      /// Log admin data
      /// </summary>
      public const string S_ADMIN_LOG = "/admin/log";

      /// <summary>
      /// Handshake/Refelect data
      /// </summary>
      public const string S_HANDSHAKE_REFLECTOR = "/handshakereflector";


      /// <summary>
      /// Starts a service, see <see cref="IServiceStart"/>
      /// </summary>
      public const string S_SERVICE_START = "/service/start";

      /// <summary>
      /// Stops a service, see <see cref="IServiceStart"/>
      /// </summary>
      public const string S_SERVICE_STOP = "/service/stop";


      /// <summary>
      /// Any data send by this channels tells the admin-process to shutdown
      /// </summary>
      public const string S_SHUTDOWN_ADMIN = "/shutdown/admin";

      /// <summary>
      /// Keyword for <see cref="S_SHUTDOWN_ADMIN"/>
      /// </summary>
      public const string SHUTDOWN_KEYWORD = "EXEC_SHUTDOWN";
   }
}
