using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Config
{
   /// <summary>
   /// Configuration for controlled Service
   /// </summary>
   public class ServiceConfig
   {
      /// <summary>
      /// Name of the Service that is used (use "sc query")
      /// </summary>
      public string ServiceName { get; set; }
   }
}
