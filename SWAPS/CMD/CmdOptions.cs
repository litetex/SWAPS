using System.Collections.Generic;
using CommandLine;

namespace SWAPS.CMD
{
   /// <summary>
   /// Possible options that can be used when calling over commandline
   /// </summary>
   public class CmdOptions
   {
      [Option('l', "logfile", Default = false, HelpText = "Logs into ./logs")]
      public bool LogToFile { get; set; } = false;

      [Option('v', "verbose", Default = false, HelpText = "Log more stuff")]
      public bool Verbose { get; set; } = false;

      [Option("showServerConsole", HelpText = "Shows the server console")]
      public bool ShowServerConsole { get; set; } 

      [Option("useUnencryptedCom", HelpText = "Uses no encryption for the protection")]
      public bool UseUnencryptedCom { get; set; }

      #region JSON based Config
      [Option('c', "config", HelpText = "path to the configuration file; if not set: using default internal config")]
      public string ConfigPath { get; set; } = null;

      [Option("genconf", HelpText = "generates default config in mentioned path; if not set: using default internal config")]
      public string ConfigGenerationPath { get; set; } = null;
      #endregion JSON based Config
   }
}
