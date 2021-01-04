using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace SWAPS.CMD
{
   [Verb("genconfig")]
   public class GenConfigOptions : AbstractCmdOptions
   {
      [Option("path", HelpText = "Generates the config file in mentioned path; e.g. dir\\abc.config")]
      public string ConfigGenerationPath { get; set; } = null;
   }
}
