using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Config
{
   /// <summary>
   /// Configuration for launched process
   /// </summary>
   public class ProcessConfig
   {
      public string WorkDir { get; set; }

      public string FilePath { get; set; }

      public string Args { get; set; }

      public TimeSpan? Timeout { get; set; } = null;
   }
}
