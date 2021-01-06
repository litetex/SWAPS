using CoreFramework.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Config
{
   public class Configuration : JsonConfig
   {
      // Increment this on major changes
      public const int CURRENT_VERSION = 2;

      public int Version { get; set; }

      public string Name { get; set; }

      public List<ServiceConfig> ServiceConfigs { get; set; } = new List<ServiceConfig>();

      /// <summary>
      /// Config for subprocess/program
      /// </summary>
      public List<ProcessConfig> ProcessConfigs { get; set; } = new List<ProcessConfig>();

      /// <summary>
      /// If the subprocess/program updates itself, it sometimes will try to uninstall the service --> ServiceNotFound (1060)
      /// </summary>
      public bool CrashOnUpdateServiceNotFound { get; set; } = false;


      /// <summary>
      /// Timeout until the service is started
      /// </summary>
      public TimeSpan ServiceStartTimeout { get; set; } = TimeSpan.FromSeconds(10);

      /// <summary>
      /// Delay after the service got started, used for waiting, until the service is fully operational
      /// </summary>
      public TimeSpan ServiceProperlyStartedDelay { get; set; } = TimeSpan.FromSeconds(0);

      /// <summary>
      /// Time to wait to shutdown the service after the launched process was stopped
      /// </summary>
      public TimeSpan ServiceShutdownDelay { get; set; } = TimeSpan.FromSeconds(1);

      /// <summary>
      /// Time in Seconds after beeing finished until the window closes
      /// </summary>
      public TimeSpan StayingOpenBeforeEnding { get; set; } = TimeSpan.FromSeconds(0.5);
   }
}
