using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Config
{
   /// <summary>
   /// Configuration for controlled Service
   /// </summary>
   public class ServiceConfig
   {
      /// <summary>
      /// Name of the Service that is used (use "sc query")
      /// </summary>
      public string ServiceName { get; set; }

      /// <summary>
      /// If the subprocess/program updates itself, it sometimes will try to uninstall the service --> ServiceNotFound (1060)
      /// </summary>
      public bool? CrashOnUpdateServiceNotFound { get; set; } = null;

      /// <summary>
      /// Timeout until the service is started
      /// </summary>
      public TimeSpan? StartTimeout { get; set; } = null;
   }
}
