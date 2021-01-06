using System;
using System.Collections.Generic;
using System.Text;
using SWAPS.CMD;
using SWAPS.Update;

namespace SWAPS.StartUp
{
   public class UpdateStartUp : AbstractStartUp<UpdateCmdOptions>
   {
      public UpdateStartUp(UpdateCmdOptions cmdOptions) : base(cmdOptions)
      {
      }

      public override void Run()
      {
         Log.Info("Trying to update now");

         var updater = new Updater(byPassUpdateLoopProtection: CmdOptions.ByPassUpdateLoopProtection);
         updater.ForceUpdateNow();
      }
   }
}
