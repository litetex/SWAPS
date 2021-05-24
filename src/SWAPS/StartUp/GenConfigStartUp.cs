using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SWAPS.CMD;
using SWAPS.Config;
using SWAPS.Update;

namespace SWAPS.StartUp
{
   public class GenConfigStartUp : AbstractStartUp<GenConfigOptions>
   {
      private Configuration Config { get; set; } = new Configuration();

      public GenConfigStartUp(GenConfigOptions cmdOptions) : base(cmdOptions)
      {
      }

      public override void Run()
      {
         Log.Info("MODE: Write Configfile");

         WriteConfigFile();
      }

      protected void WriteConfigFile()
      {
         Log.Info("Writing config file");

         if (!string.IsNullOrWhiteSpace(CmdOptions.ConfigGenerationPath))
            Config.Config.SavePath = CmdOptions.ConfigGenerationPath;

         
         Config.Version = Configuration.CURRENT_VERSION; 
         Config.Services.Configs.Add(new ServiceConfig()
         {
            ServiceName = "ServiceName"
         });

         Config.Processes.Configs.Add(new ProcessConfig()
         {
            FilePath = @"dir\executable.exe",
            Args = "-ip 127.0.0.1",
            WorkDir = "dir",
         });

         Log.Info($"Saving '{Config.Config.SavePath}'");
         Config.Save();

         Log.Info($"Saving: success");
      }
   }
}
