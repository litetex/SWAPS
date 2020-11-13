using System;
using System.Collections.Generic;
using System.Text;

namespace SWAPS.Shared.Com.Admin
{
   public class AdminComConfig
   {
      public bool LogToFile { get; set; } = false;


      public TimeSpan StartInactivityShutdownTimeout { get; set; } = TimeSpan.FromSeconds(5);

      public ushort ComPort { get; set; }

      public int ParentPID { get; set; }


      // TODO: No hardcode
      public string Username { get; set; } = "user";

      // TODO: No hardcode
      public string Password { get; set; } = "pw";

      public string ServerCertPublicKey { get; set; } = "todo";


      public string CreateCMDArgs =>
         (LogToFile ? $"--{nameof(LogToFile).ToLowerInvariant()}" : "") +
         $"--{nameof(StartInactivityShutdownTimeout).ToLowerInvariant()} {StartInactivityShutdownTimeout.TotalMilliseconds} " +
         $"--{nameof(ComPort).ToLowerInvariant()} {ComPort} " +
         $"--{nameof(ParentPID).ToLowerInvariant()} {ParentPID} " +
         $"--{nameof(Username).ToLowerInvariant()} {Username} " +
         $"--{nameof(Password).ToLowerInvariant()} {Password} " +
         $"--{nameof(ServerCertPublicKey).ToLowerInvariant()} {ServerCertPublicKey} " +
         "";
   }
}
