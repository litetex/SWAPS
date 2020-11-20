using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Timers;

namespace SWAPS
{
   public class ProcessAliveChecker
   {
      private int PID { get; set; }

      private Timer CheckAliveTimer { get; set; }

      public ProcessAliveChecker(int pid, TimeSpan interval, Action pidDeathAction, bool stopTimerOnProcessDeath = true)
      {
         PID = pid;

         CheckAliveTimer = new Timer(interval.TotalMilliseconds)
         {
            AutoReset = true,
            Enabled = true
         };
         CheckAliveTimer.Elapsed += (s, ev) =>
         {
            if (CheckIfStarterPIDAlive())
               return;

            if (stopTimerOnProcessDeath)
               Stop();

            pidDeathAction();
         };
      }

      public void Start()
      {
         CheckAliveTimer.Start();
      }

      public void Stop()
      {
         CheckAliveTimer.Stop();
      }

      private bool CheckIfStarterPIDAlive()
      {
         return CheckIfStarterPIDAlive(PID);
      }

      public static bool CheckIfStarterPIDAlive(int pid)
      {
         try
         {
            return Process.GetProcessById(pid) != null;
         }
         catch
         {
            return false;
         }
      }
   }
}
