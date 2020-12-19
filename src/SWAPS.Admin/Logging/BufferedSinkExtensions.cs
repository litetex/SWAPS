using System;
using System.Collections.Generic;
using System.Text;
using Serilog;
using Serilog.Configuration;
using Serilog.Formatting.Display;
using Serilog.Sinks.SystemConsole.Output;

namespace SWAPS.Admin.Logging
{
   public static class BufferedSinkExtensions
   {
      const string DefaultOutputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

      public static LoggerConfiguration Buffered(
             this LoggerSinkConfiguration loggerConfiguration,
             Action<string> writer,
             Func<bool> writerAvailable,
             string outputTemplate = DefaultOutputTemplate,
             IFormatProvider formatProvider = null,
             Action<List<string>> massWriter = null,
             int? bufferMaxCount = null)
      {
         return loggerConfiguration.Sink(new BufferedSink(writer, writerAvailable, new MessageTemplateTextFormatter(outputTemplate, formatProvider), massWriter, bufferMaxCount));
      }
   }
}
