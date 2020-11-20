using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Shared.Com.Admin
{
   public class AdminComConfig
   {
      public virtual bool LogToFile { get; set; } = false;

      public virtual bool Verbose { get; set; } = false;

      public virtual bool ShowServerConsole { get; set; } = false;


      public virtual TimeSpan StartInactivityShutdownTimeout { get; set; } = TimeSpan.FromSeconds(30);

      public virtual ushort ComPort { get; set; }

      public virtual int ParentPID { get; set; }


      public virtual string Username { get; set; }

      public virtual string Password { get; set; }

      public virtual string ServerCertPublicKey { get; set; } = "todo";


      public string CreateCMDArgs =>
         (LogToFile ? $"--{nameof(LogToFile).ToLowerInvariant()} " : "") +
         (Verbose ? $"--{nameof(Verbose).ToLowerInvariant()} " : "") +
         (ShowServerConsole ? $"--{nameof(ShowServerConsole).ToLowerInvariant()} " : "") +
         $"--{nameof(StartInactivityShutdownTimeout).ToLowerInvariant()} {StartInactivityShutdownTimeout.TotalMilliseconds} " +
         $"--{nameof(ComPort).ToLowerInvariant()} {ComPort} " +
         $"--{nameof(ParentPID).ToLowerInvariant()} {ParentPID} " +
         $"--{nameof(Username).ToLowerInvariant()} {Username} " +
         $"--{nameof(Password).ToLowerInvariant()} {Password} " +
         $"--{nameof(ServerCertPublicKey).ToLowerInvariant()} {ServerCertPublicKey} " +
         "";
   }
}
