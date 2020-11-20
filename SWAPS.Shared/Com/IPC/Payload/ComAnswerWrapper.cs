using System;

namespace SWAPS.Shared.Com.IPC.Payload
{
   public class ComAnswerWrapper<P> : ComMessageWrapper<P>
   {
      public Guid OriginalID { get; set; }
   }
}
