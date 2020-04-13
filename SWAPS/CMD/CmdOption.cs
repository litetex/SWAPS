using CommandLine;

namespace SWAPS.CMD
{
   /// <summary>
   /// Possible options that can be used when calling over commandline
   /// </summary>
   public class CmdOption
   {
      [Option('l', "logfile", Default = false, HelpText = "Logs into ./logs")]
      public bool LogToFile { get; set; } = false;

      #region Generate Shortcut
      [Option('s', "shortcut", Default = false, HelpText = "Generates a shortcut; requires that the path to the config is set")]
      public bool GenerateShortcut { get; set; } = false;

      [Option("sctarget", HelpText = "[Optional] Path where shortcut should be generated, DESKTOP = Desktop of the current user")]
      public string ShortcutGenerationPath { get; set; } = null;

      [Option("scname", HelpText = "[Optional] Name of the shortcut")]
      public string ShortcutName { get; set; } = null;

      [Option("scicon", HelpText = "[Optional] Icon for the shortcut")]
      public string ShortcutIcon { get; set; } = null;

      [Option("scworkingdir", HelpText = "[Optional] Workingdirectory for shortcut")]
      public string ShortcutWorkingDir { get; set; } = null;

      #endregion Generate Shortcut

      #region JSON based Config
      [Option('c', "config", HelpText = "path to the configuration file; if not set: using default internal config")]
      public string ConfigPath { get; set; } = null;

      [Option("genconf", HelpText = "generates default config in mentioned path; if not set: using default internal config")]
      public string ConfigGenerationPath { get; set; } = null;
      #endregion JSON based Config
   }
}
