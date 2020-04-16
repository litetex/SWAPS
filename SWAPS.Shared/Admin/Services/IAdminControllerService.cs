using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace SWAPS.Shared.Admin.Services
{
   /// <summary>
   /// Contains all Services
   /// </summary>
   public interface IAdminControllerService
   {
      /// <summary>
      /// Initial Handshake
      /// </summary>
      void Handshake();

      /// <summary>
      /// Shutdowns the process
      /// </summary>
      void DoShutdown();

      /// <summary>
      /// Starts a new service instance
      /// </summary>
      /// <param name="t">Service-Type</param>
      /// <returns>CommunicatonPort</returns>
      int StartServiceInstance<T>() where T : class;

      /// <summary>
      /// Shutdowns the ServiceHost on the specific port
      /// </summary>
      /// <param name="port"></param>
      void StopServiceOnPort(int port);
   }
}
