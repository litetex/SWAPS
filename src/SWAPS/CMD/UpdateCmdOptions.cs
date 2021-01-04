using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace SWAPS.CMD
{
   [Verb("update")]
   public class UpdateCmdOptions : AbstractCmdOptions
   {
      [Option("byPassUpdateLoopProtection", HelpText = "Bypasses the updateloop protection")]
      public bool ByPassUpdateLoopProtection { get; set; }
   }
}
