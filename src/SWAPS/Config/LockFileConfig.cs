using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Config
{
   public class LockFileConfig
   {
      public bool Enabled { get; set; } = false;

      public string LockFileExtension { get; set; } = ".lock";

      public TimeSpan ReNewLockFileAfter { get; set; } = TimeSpan.FromDays(1);

      public TimeSpan LockFileInvalidAfter { get; set; } = TimeSpan.FromDays(3);
   }
}
