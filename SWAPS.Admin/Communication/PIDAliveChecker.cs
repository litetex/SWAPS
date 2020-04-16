using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace SWAPS.Admin.Communication
{
   public class PIDAliveChecker
   {
      private int PID { get; set; }

      private System.Timers.Timer CheckIfStarteIsAliveTimer { get; set; }

      public PIDAliveChecker(int pid, TimeSpan interval, Action pidDeathAction)
      {
         PID = pid;

         CheckIfStarteIsAliveTimer = new System.Timers.Timer(interval.TotalMilliseconds)
         {
            AutoReset = true,
            Enabled = true
         };
         CheckIfStarteIsAliveTimer.Elapsed += (s, ev) =>
         {
            if (CheckIfStarterPIDAlive())
               return;

            pidDeathAction();
         };
      }

      public void Start()
      {
         CheckIfStarteIsAliveTimer.Start();
      }

      public void Stop()
      {
         CheckIfStarteIsAliveTimer.Stop();
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
