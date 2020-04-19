using CoreFrameworkBase.Logging.Initalizer.Impl;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWAPS.Shared
{
   public class SWAPSDefaultLoggerInitializer : DefaultLoggerInitializer
   {
      public string PresetLogfilePath { get; set; }

      protected override string GetLogFilePath()
      {
         if (string.IsNullOrWhiteSpace(PresetLogfilePath) || !File.Exists(PresetLogfilePath))
            return base.GetLogFilePath();

         return PresetLogfilePath;
      }

      protected override void DoWriteFile(LoggerConfiguration baseConf)
      {
         baseConf.WriteTo.File(
            LogfilePath,
            shared: true,
            outputTemplate: OutputTemplateFile,
            restrictedToMinimumLevel: MinimumLogEventLevelFile ?? MinimumLogEventLevel);
      }
   }
}
