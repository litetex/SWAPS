using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Shared.Com.IPC.Payload
{
   public interface IServiceStop
   {
      /// <summary>
      /// Name of the Service to start
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      /// Crash if the Service is not found 
      /// </summary>
      public bool CrashOnServiceNotFound { get; set; }
   }
}
