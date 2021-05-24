using System;
using System.Collections.Generic;
using System.Text;
using CoreFramework.Config;

namespace SWAPS.Lockfile
{
   public class LockFile : JsonConfig
   {
      // Increment this on major changes
      public const int CURRENT_VERSION = 1;

      public int FileVersion { get; set; }

      public string AppVersion { get; set; }

      public DateTimeOffset UpdateDate { get; set; }

      public DateTimeOffset CreationDate { get; set; }

      public int PID { get; set; }
   }
}
