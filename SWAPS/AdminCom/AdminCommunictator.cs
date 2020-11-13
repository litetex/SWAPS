using SWAPS.AdminCom.Service;
using SWAPS.Shared;
using SWAPS.Shared.Com.Admin;
using SWAPS.Shared.Com.IPC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp.Server;

namespace SWAPS.AdminCom
{
   public class AdminCommunictator
   {
      private AdminComConfig AdminComConfig { get; set; } = new AdminComConfig();

      private HandshakeWithinTimeout HandshakeWithinTimeout { get; set; } = new HandshakeWithinTimeout();

      private WebSocketServer Server { get; set; }

      private X509Certificate2 ServerCert { get; set; }

      private int? AdminProcessID { get; set; }

      private bool Stopped { get; set; } = false;
      private readonly object lockStop = new object();

      public AdminCommunictator(bool logToFile)
      {
         AdminComConfig.LogToFile = logToFile;
      }

      public void Start()
      {
         Log.Info("Start");
         Stopped = false;

         AppDomain.CurrentDomain.ProcessExit += ProcessExit;

         AdminComConfig.ParentPID = Process.GetCurrentProcess().Id;

         LaunchWebSocketServer();

         StartAdminProcess();

         HandshakeWithinTimeout.StartTimeout(AdminComConfig.StartInactivityShutdownTimeout, () =>
         {
            Log.Error("No handshake from Admin-process within timeout, stopping...");
            Stop();
         });
      }

      private void LaunchWebSocketServer()
      {
         CreateSelfSignedCert();

         AdminComConfig.ComPort = (ushort)NetworkUtil.GetFreeTcpPort();

         Server = new WebSocketServer(AdminComConfig.ComPort, true);
         // Auth
         Server.AuthenticationSchemes = WebSocketSharp.Net.AuthenticationSchemes.Basic;
         Server.UserCredentialsFinder = id =>
         {
            if (id.Name != AdminComConfig.Username)
               return null;

            return new WebSocketSharp.Net.NetworkCredential(id.Name, AdminComConfig.Password);
         };

         // Sec
         Server.SslConfiguration.ServerCertificate = ServerCert;
         AdminComConfig.ServerCertPublicKey = Convert.ToBase64String(ServerCert.Export(X509ContentType.Cert));

         InitServices();

         Log.Info($"Launching WebsocketServer on Port {AdminComConfig.ComPort}");
         Server.Start();
         Log.Info($"Started Server");
      }

      private void InitServices()
      {
         Server.AddWebSocketService<WSReflector>(ComServices.REFLECTOR);
         Server.AddWebSocketService<WsHandshake>(ComServices.HANDSHAKE, () => new WsHandshake(HandshakeWithinTimeout));

         Server.AddWebSocketService<WSServiceStart>(ComServices.SERVICE_START);

         Server.AddWebSocketService<WSShutdownAdmin>(ComServices.SHUTDOWN_ADMIN);
      }

      private void CreateSelfSignedCert()
      {
         Log.Info("Creating cert");
         var ecdsa = ECDsa.Create(); // generate asymmetric key pair
         // TODO
         var req = new CertificateRequest("cn=foobar", ecdsa, HashAlgorithmName.SHA256);
         ServerCert = req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddYears(5));

         Log.Info("Created cert");
      }

      private void StartAdminProcess()
      {
         Log.Info("Starting admin process");
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
               Arguments = AdminComConfig.CreateCMDArgs,
               Verb = "runas",
               UseShellExecute = true,
            }
         };

         p.Start();
         Log.Info($"Started '{p.StartInfo.FileName} {p.StartInfo.Arguments}' with PID {p.Id}");

         AdminProcessID = p.Id;
      }

      private void SendTerminateToAdminProcess()
      {
         Log.Info("Ordering admin process to shutdown");
         Server.WebSocketServices[ComServices.SHUTDOWN_ADMIN].Sessions.Broadcast("EXEC_SHUTDOWN");
      }

      private void TerminateAdminProcess()
      {
         try
         {
            SendTerminateToAdminProcess();

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

            Log.Info("Admin process took to long to exit; Killing it...");
            adminProcess.Kill();

            if (!adminProcess.WaitForExit(2000))
               throw new InvalidOperationException("Sent admin process kill command but is still running");
         }
         catch (Exception ex)
         {
            Log.Error("Failed to shutdown Admin-Process", ex);
         }
      }

      private void ShutdownWebSocketServer()
      {
         try
         {
            Server.Stop();
         }
         catch (Exception ex)
         {
            Log.Error("Failed to shutdown WebSocket-Server", ex);
         }
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
               TerminateAdminProcess();

               ShutdownWebSocketServer();
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
