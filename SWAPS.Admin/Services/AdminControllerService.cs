using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SWAPS.Admin.Communication;
using SWAPS.Admin.Service;
using SWAPS.Shared.Admin;
using SWAPS.Shared.Admin.Services;

namespace SWAPS.Admin.Services
{
   /// <summary>
   /// Contains all Services
   /// </summary>
   public class AdminControllerService : IAdminControllerService
   {
      private readonly IServiceControllerService _serviceControllerService;

      public AdminControllerService(
         IServiceControllerService serviceControllerService)
      {
         _serviceControllerService = serviceControllerService;
      }

      public IServiceControllerService ServiceControllerService { get => _serviceControllerService; }
      public IServiceControllerService GetServiceControllerService()
      {
         return _serviceControllerService;
      }


      public void DoShutdown()
      {
         Log.Info("Executing shutdown");
         StartUp.Communicator.Stop();
      }


      public void Handshake()
      {
         StartUp.Communicator.Handshake();
      }

      public int StartServiceInstance<T>() where T : class
      {
         return StartUp.Communicator.CreateNewServiceHostWithService<T>();
      }

      public void StopServiceOnPort(int port)
      {
         StartUp.Communicator.StopServiceHost(port);
      }
   }
}
