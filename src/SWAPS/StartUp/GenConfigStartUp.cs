using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SWAPS.CMD;
using SWAPS.Config;
using SWAPS.Persistence;
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

         var configPath = !string.IsNullOrWhiteSpace(CmdOptions.ConfigGenerationPath) ? CmdOptions.ConfigGenerationPath : ConfigPersister.DEFAULT_SAVEPATH;
         
         Config.Version = Configuration.CURRENT_VERSION; 
         Config.ServiceConfigs.Add(new ServiceConfig()
         {
            ServiceName = "ServiceName"
         });

         Config.ProcessConfigs.Add(new ProcessConfig()
         {
            FilePath = @"dir\executable.exe",
            Args = "-ip 127.0.0.1",
            WorkDir = "dir",
         });

         Log.Info($"Saving '{configPath}'");
         ConfigPersister.Instance.Save(Config, configPath);

         Log.Info($"Saving: success");
      }
   }
}
