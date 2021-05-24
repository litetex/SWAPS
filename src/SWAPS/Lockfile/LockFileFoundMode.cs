using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Lockfile
{
   /// <summary>
   /// Describes what is done, when a lockfile is found for the configuration
   /// </summary>
   public enum LockFileFoundMode
   {
      /// <summary>
      /// Terminates the program
      /// </summary>
      Terminate,
      /// <summary>
      /// Ignores lockfiles and starts anyway<br/>
      /// May result in unexpected behavior
      /// </summary>
      Ignore,
      /// <summary>
      /// Asks the user (via the console) what should be done
      /// </summary>
      AskUser
   }
}
