using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Admin.Config
{
   public class ComConfig
   {
      public TimeSpan StarterTimeout { get; set; }

      public int StarterPID { get; set; }

      public ushort ComTCPPort { get; set; }
   }
}
