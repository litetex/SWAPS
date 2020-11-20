using System;
using CommandLine;
using SWAPS.Shared.Com.Admin;

namespace SWAPS.Admin.CMD
{
   /// <summary>
   /// Possible options that can be used when calling over commandline
   /// </summary>
   public class CmdOptions : AdminComConfig
   {

      [Option(nameof(LogToFile))]
      public override bool LogToFile { get => base.LogToFile; set => base.LogToFile = value; }

      [Option(nameof(Verbose))]
      public override bool Verbose { get => base.Verbose; set => base.Verbose = value; }

      [Option(nameof(ShowServerConsole))]
      public override bool ShowServerConsole { get => base.ShowServerConsole; set => base.ShowServerConsole = value; }


      [Option(nameof(StartInactivityShutdownTimeout), Required = true)]
      public long StartInactivityShutdownTimeoutMs { get => (long)base.StartInactivityShutdownTimeout.TotalMilliseconds; set => base.StartInactivityShutdownTimeout = TimeSpan.FromMilliseconds(value); }

      [Option(nameof(ComPort), Required = true)]
      public override ushort ComPort { get => base.ComPort; set => base.ComPort = value; }

      [Option(nameof(ParentPID), Required = true)]
      public override int ParentPID { get => base.ParentPID; set => base.ParentPID = value; }


      [Option(nameof(Username), Required = true)]
      public override string Username { get => base.Username; set => base.Username = value; }

      [Option(nameof(Password), Required = true)]
      public override string Password { get => base.Password; set => base.Password = value; }

      [Option(nameof(UnencryptedServerCom))]
      public override bool UnencryptedServerCom { get => base.UnencryptedServerCom; set => base.UnencryptedServerCom = value; }

      [Option(nameof(ServerCertPublicKey))]
      public override string ServerCertPublicKey { get => base.ServerCertPublicKey; set => base.ServerCertPublicKey = value; }

   }
}
