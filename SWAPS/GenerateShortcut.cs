using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace SWAPS
{
   public class GenerateShortcut
   {
      protected GenerateShortcutConfig Config { get; set; }

      public GenerateShortcut(GenerateShortcutConfig configuration)
      {
         Contract.Requires(configuration != null);
         Config = configuration;
      }

      public void Generate()
      {
         //WshShell wsh = new WshShell();
         //IWshRuntimeLibrary.IWshShortcut shortcut = wsh.CreateShortcut(
         //    Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\shorcut.lnk") as IWshRuntimeLibrary.IWshShortcut;
         //shortcut.Arguments = "c:\\app\\settings1.xml";
         //shortcut.TargetPath = "c:\\app\\myftp.exe";
         //// not sure about what this is for
         //shortcut.WindowStyle = 1;
         //shortcut.Description = "my shortcut description";
         //shortcut.WorkingDirectory = "c:\\app";
         //shortcut.IconLocation = "specify icon location";
         //shortcut.Save();
      }
   }

   public class GenerateShortcutConfig
   {
      public string GenerationPath { get; set; } = null;

      public string Name { get; set; } = null;

      public string Icon { get; set; } = null;

      public string WorkingDir { get; set; } = null;
   }
}
