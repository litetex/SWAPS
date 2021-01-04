using System;
using System.Collections.Generic;
using System.Text;
using CoreFramework.Config;
using SWAPS.CMD;
using SWAPS.Config;
using SWAPS.Update;
using SWAPS.Util;

namespace SWAPS.StartUp
{
   public class RunStartUp : AbstractStartUp<RunCmdOptions>
   {
      protected Configuration Config { get; set; } = new Configuration();

      public RunStartUp(RunCmdOptions cmdOptions) : base(cmdOptions)
      {
      }

      public override void Run()
      {
         if (!CmdOptions.StartNotMinimized)
            ConsoleController.TryHideConsole();

         Log.Info("MODE: Normal start");
         ReadJsonConfig();

         using var updater = new Updater(CmdOptions.UpdateMode, CmdOptions.ByPassUpdateLoopProtection);
         updater.OnStart();

         DoStart();

         updater.OnEnd();
      }

      public void ReadJsonConfig()
      {
         Log.Info("Reading json config");

         if (!string.IsNullOrWhiteSpace(CmdOptions.ConfigPath))
            Config.Config.SavePath = CmdOptions.ConfigPath;

         Log.Info($"Loading '{Config.Config.SavePath}'");
         Config.Load(LoadFileNotFoundAction.THROW_EX);

         Log.Info($"Loading: success");
      }

      private void DoStart()
      {
         Log.Info("Starting");
         new Runner(Config, CmdOptions).Run();
         Log.Info("Done");
      }
   }
}
