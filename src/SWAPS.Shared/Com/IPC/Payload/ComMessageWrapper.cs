using System;

namespace SWAPS.Shared.Com.IPC.Payload
{
   public class ComMessageWrapper<P>
   {
      public Guid ID { get; set; }

      public P PayLoad { get; set; }
   }
}
