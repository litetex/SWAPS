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
      /// <summary>
      /// Optional
      /// </summary>
      public string Key { get; set; }

      public string WorkDir { get; set; }

      public string FilePath { get; set; }

      public string Args { get; set; }

      public TimeSpan? Timeout { get; set; } = null;

      /// <summary>
      /// If set, timeout is ignored
      /// </summary>
      public bool Async { get; set; } = false;

      /// <summary>
      /// Optional
      /// </summary>
      public List<string> DependsOn { get; set; } = new List<string>();
   }
}
