using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Updater
{
   public enum UpdateMode
   {
      /// <summary>
      /// Do not install updates
      /// </summary>
      None = 1,
      /// <summary>
      ///  Show a message on the console if a update is found
      /// </summary>
      Notice = 2,
      /// <summary>
      /// Show a message if a update is found
      /// </summary>
      Ask = 3,
      /// <summary>
      /// if a update is found download & install it (at next start)
      /// </summary>
      Always = 4
   }
}
