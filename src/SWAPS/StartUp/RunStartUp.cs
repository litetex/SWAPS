using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SWAPS.CMD;
using SWAPS.Config;
using SWAPS.Persistence;
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
         var configPath = ReadJsonConfig();

         using var updater = new Updater(CmdOptions.UpdateMode, CmdOptions.ByPassUpdateLoopProtection);
         updater.OnStart();

         var canStart = CheckConfigVersion();
         if (canStart)
            DoStart(configPath);
         else
            Log.Error("Aborting main run due to config version mismatch");

         updater.OnEnd();
      }

      private string ReadJsonConfig()
      {
         Log.Info("Reading json config");

         var configPath = !string.IsNullOrWhiteSpace(CmdOptions.ConfigPath) ? CmdOptions.ConfigPath : ConfigPersister.DEFAULT_SAVEPATH;

         Log.Info($"Loading '{configPath}'");
         Config = ConfigPersister.Instance.Load(configPath);

         Log.Info($"Loading: success");
         if(Serilog.Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            Log.Debug(ConfigPersister.Instance.SerializeToFileContent(Config));

         return configPath;
      }

      private bool CheckConfigVersion()
      {
         if (Config.Version != Configuration.CURRENT_VERSION)
         {
            Log.Warn($"Found config with version '{Config.Version}' but can only process '{Configuration.CURRENT_VERSION}' without errors");
            if (Config.Version < Configuration.CURRENT_VERSION)
               Log.Warn("Please update your configuration file; Checkout https://git.io/JLAkA");
            else if (Config.Version > Configuration.CURRENT_VERSION)
               Log.Warn("Please update the executable; Run 'SWAPS.exe update'");

            if (CmdOptions.AbortOnConfigVersionMismatch)
               return false;
         }
         return true;
      }

      private void DoStart(string configPath)
      {
         Log.Info("Starting");
         new Runner(Config, configPath, CmdOptions).Run();
         Log.Info("Done");
      }
   }
}
