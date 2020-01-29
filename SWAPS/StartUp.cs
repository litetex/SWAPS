﻿using SWAPS.CMD;
using SWAPS.Config;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace SWAPS
{
   public class StartUp
   {
      private CmdOption CmdOption { get; set; }

      private Configuration Config { get; set; } = new Configuration();

      public StartUp(CmdOption cmdOption)
      {
         CmdOption = cmdOption;
      }

      public void Start()
      {
         Contract.Requires(CmdOption != null);
         Log.Info($"Current directory is '{Directory.GetCurrentDirectory()}'");

         if (CmdOption.ConfigGenerationPath != null)
         {
            Log.Info("MODE: Write JSON Config");

            WriteJsonConfig();
            return;
         }

         Log.Info("MODE: Normal start");
         ReadJsonConfig();

         DoStart();
      }

      public void WriteJsonConfig()
      {
         Log.Info("Writing json config");

         if (!string.IsNullOrWhiteSpace(CmdOption.ConfigGenerationPath))
            Config.Config.SavePath = CmdOption.ConfigGenerationPath;

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
         Config.Load(CoreFrameworkBase.Config.JsonConfig.LoadFileNotFoundAction.THROW_EX);

         Log.Info($"Loading: success");
      }

      private void DoStart()
      {
         Log.Info("Starting");
         new Runner(Config).Run();
         Log.Info("Done");
      }
   }
}
