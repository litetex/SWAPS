using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Shared.Com.IPC.Payload
{
   public interface IComMessage
   {
      Guid ID { get; set; }
   }
}
