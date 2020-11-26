using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Update
{
   public enum UpdateMode
   {
      /// <summary>
      /// Do not install updates
      /// </summary>
      None,
      /// <summary>
      ///  Show a message on the console if a update is found
      /// </summary>
      Notice,
      /// <summary>
      ///  Show a message on the console if a update is found and bring up the window if it is minimied
      /// </summary>
      Notify,
      /// <summary>
      /// if a update is found download & install it (at next start)
      /// </summary>
      Always
   }
}
