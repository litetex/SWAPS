using CoreFramework.Config;
using SWAPS.CMD;
using SWAPS.Config;
using SWAPS.Update;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace SWAPS
{
   public class StartUp
   {
      private CmdOptions CmdOption { get; set; }

      private Configuration Config { get; set; } = new Configuration();

      public StartUp(CmdOptions cmdOption)
      {
         CmdOption = cmdOption;
      }

      public void Start()
      {
         Contract.Requires(CmdOption != null);
         Log.Info($"Current directory is '{Directory.GetCurrentDirectory()}'");

         if(CmdOption.Update)
         {
            UpdateNow();
            return;
         }
         if (CmdOption.ConfigGenerationPath != null)
         {
            Log.Info("MODE: Write Configfile");

            WriteConfigFile();
            return;
         }

         Log.Info("MODE: Normal start");
         ReadJsonConfig();

         using var updater = new Updater(CmdOption.UpdateMode, CmdOption.ByPassUpdateLoopProtection);
         updater.OnStart();

         DoStart();

         updater.OnEnd();
      }

      protected void UpdateNow()
      {
         Log.Info("Trying to update now");

         var updater = new Updater();
         updater.ForceUpdateNow();
      }

      protected void WriteConfigFile()
      {
         Log.Info("Writing config file");

         if (!string.IsNullOrWhiteSpace(CmdOption.ConfigGenerationPath))
            Config.Config.SavePath = CmdOption.ConfigGenerationPath;

         Config.ServiceConfigs.Add(new ServiceConfig()
         {
            ServiceName = "ServiceName"
         });

         Config.ProcessConfigs.Add(new ProcessConfig()
         {
            Key = "abc",
            FilePath = @"dir\executable.exe",
            Args = "-ip 127.0.0.1",
            WorkDir = "dir",
         });

         Log.Info($"Saving '{Config.Config.SavePath}'");
         Config.Save();

         Log.Info($"Saving: success");
      }

      public void ReadJsonConfig()
      {
         Log.Info("Reading json config");

         if (!string.IsNullOrWhiteSpace(CmdOption.ConfigPath))
            Config.Config.SavePath = CmdOption.ConfigPath;

         Log.Info($"Loading '{Config.Config.SavePath}'");
         Config.Load(LoadFileNotFoundAction.THROW_EX);

         Log.Info($"Loading: success");
      }

      private void DoStart()
      {
         Log.Info("Starting");
         new Runner(Config, CmdOption).Run();
         Log.Info("Done");
      }
   }
}
