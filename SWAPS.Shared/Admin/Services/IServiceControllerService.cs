using System;
using System.ServiceProcess;

namespace SWAPS.Shared.Admin.Services
{
   /// <summary>
   /// Controls Windows Services
   /// </summary>
   public interface IServiceControllerService
   {
      void StartUp(string name, TimeSpan serviceStartTimeout);

      void ShutDown(string name, bool crashOnServiceNotFound);
   }
}
