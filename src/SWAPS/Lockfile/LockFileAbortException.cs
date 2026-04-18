using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SWAPS.Lockfile
{
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
   }
}
