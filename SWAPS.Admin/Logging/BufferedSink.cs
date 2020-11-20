using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace SWAPS.Admin.Logging
{
   public class BufferedSink : ILogEventSink
   {
      private readonly object _lockBuffer = new object();
      private List<string> Buffer { get; set; } = new List<string>();

      private Action<string> Writer { get; set; }
      private Func<bool> WriterAvailable { get; set; }
      private Action<List<string>> MassWriter { get; set; }

      private int? BufferMaxCount { get; set; }

      private ITextFormatter TextFormatter { get; set; }

      public BufferedSink(
         Action<string> writer,
         Func<bool> writerAvailable,
         ITextFormatter textFormatter = null,
         Action<List<string>> massWriter = null,
         int? bufferMaxCount = null)
      {
         TextFormatter = textFormatter;
         Writer = writer;
         WriterAvailable = writerAvailable;
         MassWriter = massWriter;
         BufferMaxCount = bufferMaxCount;
      }

      public void Emit(LogEvent logEvent)
      {
         var stringWriter = new StringWriter();
         TextFormatter.Format(logEvent, stringWriter);

         var formatted = stringWriter.ToString();
         if (!WriterAvailable())
         {
            AddToBuffer(formatted);
            if (BufferMaxCount != null && Buffer.Count > BufferMaxCount)
                  throw new InvalidOperationException("Buffer is full");

            return;
         }

         if (Buffer.Count > 0)
            ClearBuffer();

         Writer(formatted);
      }

      protected void AddToBuffer(string text)
      {
         lock (_lockBuffer)
         {
            Buffer.Add(text);
         }
      }

      protected void ClearBuffer()
      {
         lock (_lockBuffer)
         {
            if (Buffer.Count == 0)
               return;

            if (MassWriter != null)
            {
               MassWriter(new List<string>(Buffer));
               Buffer.Clear();
               return;
            }

            foreach (var s in new List<string>(Buffer))
            {
               Writer(s);
               Buffer.Remove(s);
            }
         }
      }
   }
}
