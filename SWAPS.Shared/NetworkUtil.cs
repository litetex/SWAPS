using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SWAPS.Shared
{
   public static class NetworkUtil
   {
      public static int GetFreeTcpPort()
      {
         var l = new TcpListener(IPAddress.Loopback, 0);
         l.Start();

         var port = ((IPEndPoint)l.LocalEndpoint).Port;
         l.Stop();

         return port;
      }
   }
}
