using JKang.IpcServiceFramework;
using SWAPS.Shared;
using SWAPS.Shared.Admin;
using SWAPS.Shared.Admin.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SWAPS.AdminCom
{
   public class AdminCommunictator
   {
      private bool LogToFile { get; set; }

      private IpcServiceClient<IAdminControllerService> Client { get; set; }

      public IpcServiceClient<IAdminControllerService> PublicClient { get; set; }

      private int? AdminProcessID { get; set; }

      private int? AdminComPort { get; set; }

      private bool Stopped { get; set; } = false;
      private readonly object lockStop = new object();

      public AdminCommunictator(bool logToFile)
      {
         LogToFile = logToFile;
      }

      public void Start()
      {
         Log.Info("Start");
         Stopped = false;

         AppDomain.CurrentDomain.ProcessExit += ProcessExit;

         AdminComPort = NetworkUtil.GetFreeTcpPort();
         Log.Info($"Will use Port {AdminComPort}");

         Log.Info("Starting admin process");
         StartAdminProcess();

         Log.Info("Establishing connectivity with admin process");
         ConnectClient();
      }

      private void StartAdminProcess()
      {
         using var p = new Process()
         {
            StartInfo = new ProcessStartInfo()
            {
               FileName = Path.Combine(
#if !DEBUG
                  Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName),
#else
                  @"..\..\..\..\SWAPS.Admin\bin\Debug\netcoreapp3.1",
#endif
                  "SWAPS.Admin.exe"),
               Arguments = $"--comstarterpid {Process.GetCurrentProcess().Id} --comtcpport {AdminComPort}{(LogToFile ? $" -l" : "")}",
               Verb = "runas",
               UseShellExecute = true,
            }
         };

         p.Start();
         Log.Info($"Started '{p.StartInfo.FileName} {p.StartInfo.Arguments}' with PID {p.Id}");
         AdminProcessID = p.Id;

         //AdminProcess.BeginErrorReadLine();
         //AdminProcess.BeginOutputReadLine();

         //AdminProcess.OutputDataReceived += (s, ev) =>
         //{
         //   if (AdminProcess.HasExited && ev.Data != null)
         //      Log.Info($"[ADMIN] {AdminProcess.Id} >> {ev.Data}");
         //};
         //AdminProcess.ErrorDataReceived += (s, ev) =>
         //{
         //   if (AdminProcess.HasExited && ev.Data != null)
         //      Log.Error($"[ADMIN] {AdminProcess.Id} >> {ev.Data}");
         //};

      }

      private void ConnectClient()
      {
         Log.Info("Building client");
         Client = new IpcServiceClientBuilder<IAdminControllerService>()
            .UseTcp(IPAddress.Loopback, AdminComPort.Value)
            .Build();

         Log.Info("Handshaking...");
         if (!Client.InvokeAsync(s => s.Handshake()).Wait(TimeSpan.FromSeconds(3)))
            throw new System.TimeoutException("Handshake timed out");

         Log.Info("Handshake successful");

         PublicClient = Client;
      }

      private void TerminateClient()
      {
         PublicClient = null;

         if (Client == null)
         {
            Log.Warn("No client to order shutdown");
            return;
         }

         Log.Info("Ordering admin process / server to shutdown");
         Client.InvokeAsync(s => s.DoShutdown());
      }

      public void Stop()
      {
         if (Stopped)
            return;

         lock (lockStop)
         {
            if (Stopped)
               return;

            Log.Info("Stopping admin process");

            AppDomain.CurrentDomain.ProcessExit -= ProcessExit;

            try
            {
               TerminateClient();

               // AdminProcess is not accessible here anymore: there is no process associated with this instance
               if (AdminProcessID == null || Process.GetProcessById(AdminProcessID.Value) == null)
               {
                  Log.Info("Admin process is not running");
                  return;
               }

               var adminProcess = Process.GetProcessById(AdminProcessID.Value);

               Log.Info("Waiting for admin process termination");
               if (adminProcess.WaitForExit(2000))
               {
                  Log.Info("Admin process has exited");
                  return;
               }

               Log.Info("Admin process took too long to exit; Killing it!");
               adminProcess.Kill();

               Thread.Sleep(2000);
               if (Process.GetProcessById(AdminProcessID.Value) != null)
                  throw new InvalidOperationException("Sent kill message to process but still running");
            }
            catch (Exception ex)
            {
               Log.Error(ex);
            }
            finally
            {
               Stopped = true;
            }
         }
      }

      private void ProcessExit(object sender, EventArgs e)
      {
         Log.Info("Process exit; Ensuring admin process is getting killed");
         Stop();
      }
   }
}
