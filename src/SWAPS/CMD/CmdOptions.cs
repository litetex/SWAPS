using System.Collections.Generic;
using CommandLine;
using SWAPS.Update;

namespace SWAPS.CMD
{
   /// <summary>
   /// Possible options that can be used when calling over commandline
   /// </summary>
   public class CmdOptions
   {
      [Option("update", HelpText = "Install the latest available update")]
      public bool Update { get; set; }

      [Option("updatemode", Default = Updater.DEFAULT_MODE, HelpText = "Describes when updates are searched and installed")]
      public UpdateMode UpdateMode { get; set; } = Updater.DEFAULT_MODE;

      [Option("byPassUpdateLoopProtection", HelpText = "Bypasses the updateloop protection")]
      public bool ByPassUpdateLoopProtection { get; set; }

      #region Configfile
      [Option("genconf", HelpText = "Generates default config file in mentioned path")]
      public string ConfigGenerationPath { get; set; } = null;

      [Option('c', "config", HelpText = "Path to the config file; if value not set: using defaults")]
      public string ConfigPath { get; set; } = null;
      #endregion JSON based Config

      [Option('l', "logfile", Default = false, HelpText = "Writes logs as file(s) into ./logs")]
      public bool LogToFile { get; set; } = false;

      [Option("logFileRetainCount", HelpText = "The maximum number of log files that will be retained, including the current " +
         "log file. For unlimited retention, pass -1. The default is 31.")]
      public int LogFileRetainCount { get; set; } = 31;

      [Option("startNotMin", HelpText = "Starts not minimized")]
      public bool StartNotMinimized { get; set; }


      [Option('v', "verbose", Default = false, HelpText = "More logs (for debugging)")]
      public bool Verbose { get; set; } = false;

      [Option("showServerConsole", HelpText = "Shows the server console (for debugging)")]
      public bool ShowServerConsole { get; set; } 

      [Option("useUnencryptedCom", HelpText = "Uses no encryption for the communication between processes (for debugging; not recommended)")]
      public bool UseUnencryptedCom { get; set; }
   }
}
