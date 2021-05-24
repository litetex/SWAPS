using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Config
{
   public class ServicesConfig
   {
      /// <summary>
      /// If the subprocess/program updates itself, it sometimes will try to uninstall the service --> ServiceNotFound (1060)
      /// </summary>
      public bool CrashOnUpdateServiceNotFound { get; set; } = false;

      /// <summary>
      /// Timeout until the service is started
      /// </summary>
      public TimeSpan StartTimeout { get; set; } = TimeSpan.FromSeconds(10);

      /// <summary>
      /// Delay after the service got started, used for waiting, until the service is fully operational
      /// </summary>
      public TimeSpan ProperlyStartedDelay { get; set; } = TimeSpan.FromSeconds(0);

      /// <summary>
      /// Time to wait to shutdown the service after the launched process was stopped
      /// </summary>
      public TimeSpan ShutdownDelay { get; set; } = TimeSpan.FromSeconds(1);


      public List<ServiceConfig> Configs { get; set; } = new List<ServiceConfig>();
   }
}
