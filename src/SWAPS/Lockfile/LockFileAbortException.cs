using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SWAPS.Lockfile
{
   [Serializable]
   public class LockFileAbortException : Exception
   {
      public LockFileAbortException()
      {
      }

      public LockFileAbortException(string message) : base(message)
      {
      }

      public LockFileAbortException(string message, Exception innerException) : base(message, innerException)
      {
      }

      protected LockFileAbortException(SerializationInfo info, StreamingContext context) : base(info, context)
      {
      }
   }
}
