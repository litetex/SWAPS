using CommandLine;

namespace SWAPS.Admin.CMD
{
   /// <summary>
   /// Possible options that can be used when calling over commandline
   /// </summary>
   public class CmdOption
   {
      [Option('l', "logfile", Default = false, HelpText = "Logs into ./logs")]
      public bool LogToFile { get; set; } = false;

      [Option("comstarttimeout", Default = 5000, HelpText = "Timeout after start, when no communication happens (in ms)")]
      public long StartCommunicationTimeout { get; set; } = 5000;

      [Option("comstarterpid", Required = true, HelpText = "ProcessID of starter process")]
      public int StarterPID { get; set; }

      [Option("comtcpport", Required = true, HelpText = "Port for TCP Communication")]
      public ushort ComTCPPort { get; set; }

   }
}
