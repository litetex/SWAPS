using System.Collections.Generic;
using CommandLine;
using SWAPS.Update;

namespace SWAPS.CMD
{
   public abstract class AbstractCmdOptions
   {
      [Option('l', "logfile", Default = false, HelpText = "Writes logs as file(s) into ./logs")]
      public bool LogToFile { get; set; } = false;

      [Option("logFileRetainCount", HelpText = "The maximum number of log files that will be retained, including the current " +
         "log file. For unlimited retention, pass -1. The default is 31.")]
      public int LogFileRetainCount { get; set; } = 31;

      [Option('v', "verbose", Default = false, HelpText = "More logs (for debugging)")]
      public bool Verbose { get; set; } = false;
   }
}
