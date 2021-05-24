using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;
using SWAPS.Lockfile;
using SWAPS.Update;

namespace SWAPS.CMD
{
   [Verb("run", isDefault: true)]
   public class RunCmdOptions : AbstractCmdOptions
   {
      #region Configfile
      [Option('c', "config", HelpText = "Path to the config file; if value not set: using defaults")]
      public string ConfigPath { get; set; } = null;

      [Option("abortOnConfigVersionMismatch", HelpText = "Abort on configuration file version mismatch")]
      public bool AbortOnConfigVersionMismatch { get; set; }

      [Option("lockFileFoundMode", HelpText = "Describes what is done, when a lockfile is found for the configuration")]
      public LockFileFoundMode LockFileFoundMode { get; set;} = LockFileFoundMode.Terminate;

      #endregion Configfile

      [Option("startNotMin", HelpText = "Starts not minimized")]
      public bool StartNotMinimized { get; set; }

      [Option("showServerConsole", HelpText = "Shows the server console (for debugging)")]
      public bool ShowServerConsole { get; set; }

      [Option("useUnencryptedCom", HelpText = "Uses no encryption for the communication between processes (for debugging; not recommended)")]
      public bool UseUnencryptedCom { get; set; }

      #region Updater
      [Option("updatemode", Default = Updater.DEFAULT_MODE, HelpText = "Describes when updates are searched and installed")]
      public UpdateMode UpdateMode { get; set; } = Updater.DEFAULT_MODE;

      [Option("byPassUpdateLoopProtection", HelpText = "Bypasses the updateloop protection")]
      public bool ByPassUpdateLoopProtection { get; set; }

      #endregion Updater
   }
}
