using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SWAPS.Util
{
   public static class ProcessUtil
   {
      public static bool RunProcess(ProcessStartInfo processStartInfo, TimeSpan? timeout = null)
      {
         using Process p = new Process()
         {
            StartInfo = processStartInfo
         };

         bool processAvaible = true;

         p.Start();
         Log.Info($"Started '{p.StartInfo.FileName} {p.StartInfo.Arguments}' with PID {p.Id}{(!string.IsNullOrWhiteSpace(p.StartInfo.WorkingDirectory) ? $" in '{p.StartInfo.WorkingDirectory}'" : "")}");

         if(processStartInfo.RedirectStandardError)
            p.BeginErrorReadLine();
         if (processStartInfo.RedirectStandardOutput)
            p.BeginOutputReadLine();

         p.OutputDataReceived += (s, ev) =>
         {
            if (processAvaible && ev.Data != null)
               Log.Info($"{p.Id} >> {ev.Data}");
         };
         p.ErrorDataReceived += (s, ev) =>
         {
            if (processAvaible && ev.Data != null)
               Log.Error($"{p.Id} >> {ev.Data}");
         };

         if (timeout != null)
         {
            if (!p.WaitForExit((int)timeout.Value.TotalMilliseconds))
            {
               Log.Error($"{p.Id} Timed out[{timeout}]! Killing it");

               p.Kill();

               Log.Info($"Killed {p.Id}");

               throw new TimeoutException($"{p.Id} Timed out[{timeout}]");
            }
         }
         else
         {
            p.WaitForExit();
         }

         var msg = $"{p.Id} finished with ExitCode={p.ExitCode}";
         if (p.ExitCode == 0)
            Log.Info(msg);
         else
            Log.Warn(msg);

         processAvaible = false;

         return p.ExitCode == 0;
      }
   }
}
