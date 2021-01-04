using System;
using System.Collections.Generic;
using System.Text;
using SWAPS.CMD;

namespace SWAPS.StartUp
{
   public abstract class AbstractStartUp<C> where C : AbstractCmdOptions
   {
      protected C CmdOptions { get; set; }

      protected AbstractStartUp(C cmdOptions)
      {
         CmdOptions = cmdOptions;
      }

      public abstract void Run();
   }
}
