using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace SWAPS.CMD
{
   [Verb("genconfig")]
   public class GenConfigOptions : AbstractCmdOptions
   {
      [Option("path", HelpText = "Generates default config file in mentioned path")]
      public string ConfigGenerationPath { get; set; } = null;
   }
}
