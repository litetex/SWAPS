using CoreFramework.Logging.Initalizer.Impl;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWAPS.Shared
{
#pragma warning disable S101 // Types should be named in PascalCase
   public class SWAPSDefaultLoggerInitializer : DefaultLoggerInitializer
#pragma warning restore S101 // Types should be named in PascalCase
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
            outputTemplate: Config.OutputTemplateFile,
            restrictedToMinimumLevel: Config.MinimumLogEventLevelFile ?? Config.MinimumLogEventLevel);
      }
   }
}
