using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Shared.Com.IPC.Payload
{
   public class ServiceStart
   {
      /// <summary>
      /// Name of the Service to start
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      /// Timeout of the service
      /// </summary>
      public TimeSpan Timeout { get; set; }
   }
}
