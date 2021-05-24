using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Config
{
   public class ProcessesConfig
   {
      /// <summary>
      /// Config for subprocess/program
      /// </summary>
      public List<ProcessConfig> Configs { get; set; } = new List<ProcessConfig>();
   }
}
