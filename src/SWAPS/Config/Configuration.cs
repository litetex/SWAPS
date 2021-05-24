using CoreFramework.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Config
{
   public class Configuration : JsonConfig
   {
      // Increment this on major changes
      public const int CURRENT_VERSION = 3;

      public int Version { get; set; }

      public string Name { get; set; }

      public LockFileConfig LockFile { get; set; } = new LockFileConfig();

      public ServicesConfig Services { get; set; } = new ServicesConfig();

      public ProcessesConfig Processes { get; set; } = new ProcessesConfig();

      /// <summary>
      /// Time in Seconds after beeing finished until the window closes
      /// </summary>
      public TimeSpan StayingOpenBeforeEnding { get; set; } = TimeSpan.FromSeconds(0.5);
   }
}
