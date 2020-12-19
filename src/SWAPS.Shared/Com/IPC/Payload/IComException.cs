using System;

namespace SWAPS.Shared.Com.IPC.Payload
{
   public interface IComException
   {
      public Guid ID { get; set; }

      public Guid OriginalID { get; set; }
   }
}
