using JKang.IpcServiceFramework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SWAPS.Admin.Communication
{
   public class ComServiceHost
   {
      public IIpcServiceHost Host { get; set; }

      public Type Service { get; set; }

      public int ComPort { get; set; }

      public string Name { get; set; }

      public CancellationTokenSource CancelTokenSource { get; set; }
   }
}
