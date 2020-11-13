using SWAPS.Admin.CMD;
using SWAPS.Admin.Communication;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace SWAPS.Admin
{
   public class StartUp
   {
      public static Communicator Communicator { get; private set; }

      private CmdOptions CmdOption { get; set; }

      public StartUp(CmdOptions cmdOption)
      {
         CmdOption = cmdOption;
      }

      public void Start()
      {
         Contract.Requires(CmdOption != null);
         Log.Info($"Current directory is '{Directory.GetCurrentDirectory()}'");

         DoStart();
      }

      private void DoStart()
      {
         Log.Info("Starting");
         Communicator = new Communicator(new Config.ComConfig()
         {
            StarterTimeout = TimeSpan.FromMilliseconds(CmdOption.StartCommunicationTimeout),
            ComTCPPort = CmdOption.ComTCPPort,
            StarterPID = CmdOption.StarterPID
         });
         Communicator.Run();
         Log.Info("Done");
      }
   }
}
