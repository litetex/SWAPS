using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using SWAPS.Admin.Services;
using System.Threading;
using System.Linq;
using SWAPS.Shared;
using SWAPS.Shared.Com.Admin;
using WebSocketSharp;
using SWAPS.Shared.Com.IPC;

namespace SWAPS.Admin.Communication
{
   public class Communicator
   {
      private AdminComConfig Config { get; set; }

      private ProcessAliveChecker StarterPIDAliveChecker { get; set; }

      private List<WebSocket> RegisteredWebsockets { get; set; } = new List<WebSocket>();


      private WebSocket WebSocketShutdownMonitor { get; set; }

      private WebSocket WebSocketLog { get; set; }


      private TaskCompletionSource<bool> Stopped { get; set; } = new TaskCompletionSource<bool>();

      private readonly object lockStop = new object();

      public Communicator(AdminComConfig configuration)
      {
         Contract.Requires(configuration != null);
         Config = configuration;
      }

      public void Run()
      {
         Init();

         RunShutdownMonitorService().Wait();
         LinkServices().Wait();
         Handshake().Wait();
         TransmitLogs().Wait();

         WaitForStop();
      }

      protected void Init()
      {
         StarterPIDAliveChecker = new ProcessAliveChecker(Config.ParentPID, TimeSpan.FromSeconds(5), () =>
         {
            Log.Error($"Unable to locate PID={Config.ParentPID} of parent-process, assuming crash; Stopping...");
            Stop();
         });

         Log.Info($"Doing inital check if starter process[PID={Config.ParentPID}] is alive");
         if (!ProcessAliveChecker.CheckIfStarterPIDAlive(Config.ParentPID))
            throw new ArgumentException($"{nameof(Config.ParentPID)}={Config.ParentPID} not found!");

         Log.Info($"Starting {nameof(StarterPIDAliveChecker)}; StarterPID={Config.ParentPID}");
         StarterPIDAliveChecker.Start();
      }

      protected async Task RunShutdownMonitorService()
      {
         Log.Info("Starting shutdown monitor (SM) service");
         WebSocketShutdownMonitor = CreateWebSocket(ComServices.S_SHUTDOWN_ADMIN);
         WebSocketShutdownMonitor.OnOpen += (s, ev) =>
         {
            Log.Debug($"SM: onOpen");
         };

         WebSocketShutdownMonitor.OnMessage += (s, ev) =>
         {
            if (ComServices.SHUTDOWN_KEYWORD.Equals(ev.Data))
            {
               Log.Info("SM: Received shutdown keyword");
               ShutdownEvent();
               return;
            }

            Log.Warn($"SM: Received unknown message on shutdown: '{ev.Data}'");
         };

         WebSocketShutdownMonitor.OnClose += (s, ev) =>
         {
            if (Stopped.Task.IsCompleted)
               return;

            Log.Error($"SM: was closed! Code={ev.Code} Reason='{ev.Reason}' WasClean={ev.WasClean}");
            ShutdownEvent();
         };

         WebSocketShutdownMonitor.OnError += (s, ev) =>
         {
            if (Stopped.Task.IsCompleted)
               return;

            Log.Error($"SM: error", ev.Exception);
            ShutdownEvent();
         };

         using (var timeoutCancellationTokenSource = new CancellationTokenSource())
         {
            var connectTask = Task.Run(() =>
            {
               try
               {
                  WebSocketShutdownMonitor.Connect();
               }
               catch(Exception ex)
               {
                  Log.Error("Connection error", ex);
               }
            });

            if (await Task.WhenAny(Task.Delay(TimeSpan.FromSeconds(5), timeoutCancellationTokenSource.Token), connectTask) == connectTask)
            {
               timeoutCancellationTokenSource.Cancel();
               await connectTask;
            }
            else
               throw new TimeoutException("Failed to connect to shutdown in time");
         }

         Log.Info("SM started successfully");
      }

      protected async Task LinkServices()
      {
         using var timeoutCancellationTokenSource = new CancellationTokenSource();

         var linkTask = Task.Run(() => new ServiceLinker(this).LinkServices());

         if (await Task.WhenAny(Task.Delay(TimeSpan.FromSeconds(5), timeoutCancellationTokenSource.Token), linkTask) == linkTask)
         {
            timeoutCancellationTokenSource.Cancel();
            await linkTask;
         }
         else
            throw new TimeoutException("Linking Services timed out");
      }

      protected async Task Handshake()
      {
         var timeout = Config.StartInactivityShutdownTimeout;

         Log.Info("Checking handshake reflector (HR)");

         var tcs = new TaskCompletionSource<string>();
         var randomStr = RandomStringGen.RandomString(64);

         using (var wsHRCheck = CreateWebSocket(ComServices.S_HANDSHAKE_REFLECTOR))
         {
            wsHRCheck.OnOpen += (s, ev) =>
            {
               Log.Debug("HR: onOpen");
               wsHRCheck.Send(randomStr);
            };

            wsHRCheck.OnMessage += (s, ev) =>
            {
               Log.Debug($"HR: onMessage: {ev.Data}");
               tcs.SetResult(ev.Data);
            };

            wsHRCheck.OnClose += (s, ev) =>
            {
               Log.Debug($"HR: onClose: Code={ev.Code} Reason='{ev.Reason}' WasClean={ev.WasClean}");
               if (tcs.Task.IsCompleted)
                  return;

               tcs.SetException(new InvalidOperationException($"HR socket was closed unexpectely; Code={ev.Code} Reason='{ev.Reason}' WasClean={ev.WasClean}"));
            };

            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {
               var connectTask = Task.Run(() => wsHRCheck.Connect());

               var connectedAndReceivedFeedbackTask = Task.WhenAll(tcs.Task, connectTask);
               var completedTask = await Task.WhenAny(Task.Delay(timeout, timeoutCancellationTokenSource.Token), connectedAndReceivedFeedbackTask);
               if (completedTask == connectedAndReceivedFeedbackTask)
               {
                  timeoutCancellationTokenSource.Cancel();
                  await connectedAndReceivedFeedbackTask;
               }
               else
                  throw new TimeoutException("HR Timeout");

            }
         }

         if (!tcs.Task.IsCompletedSuccessfully)
            throw new InvalidOperationException("HR did not completed successfully", tcs.Task.Exception);
         if (!randomStr.Equals(tcs.Task.Result))
            throw new InvalidOperationException($"HR returned wrong data; expected '{randomStr}' got '{tcs.Task.Result}'");

         Log.Info("Handshake successful/Reflector operational");
      }

      protected async Task TransmitLogs()
      {
         Log.Info("Starting logging (LM) service");
         WebSocketLog = CreateWebSocket(ComServices.S_ADMIN_LOG);
         WebSocketLog.OnOpen += (s, ev) =>
         {
            Log.Debug($"LM: onOpen");
            Program.Writer = s => WebSocketLog.Send(s.TrimEnd());
            Program.WriterAvailable = () => WebSocketLog.ReadyState == WebSocketState.Open;
         };

         WebSocketLog.OnMessage += (s, ev) =>
         {
            Log.Debug($"LM: onMessage: '{ev.Data}'");
         };

         WebSocketLog.OnClose += (s, ev) =>
         {
            Log.Debug($"LM: was closed! Code={ev.Code} Reason='{ev.Reason}' WasClean={ev.WasClean}");
         };

         WebSocketLog.OnError += (s, ev) =>
         {
            if (Stopped.Task.IsCompleted)
               return;

            Log.Error($"LM: error", ev.Exception);
         };

         using (var timeoutCancellationTokenSource = new CancellationTokenSource())
         {
            var connectTask = Task.Run(() =>
            {
               try
               {
                  WebSocketLog.Connect();
               }
               catch (Exception ex)
               {
                  Log.Error("Connection error", ex);
               }
            });

            if (await Task.WhenAny(Task.Delay(TimeSpan.FromSeconds(5), timeoutCancellationTokenSource.Token), connectTask) == connectTask)
            {
               timeoutCancellationTokenSource.Cancel();
               await connectTask;
            }
            else
               throw new TimeoutException("Failed to connect to shutdown in time");
         }

         Log.Info("LM started successfully");
      }

      protected void ShutdownEvent()
      {
         if (Stopped.Task.IsCompletedSuccessfully)
            return;

         Stopped.SetResult(true);
         WebSocketShutdownMonitor.Close();
         Log.Info("Closed SM");
      }

      protected void WaitForStop()
      {
         Log.Info("Waiting for stop");
         Stopped.Task.Wait();
         Log.Info("Stop occured!");
      }

      public WebSocket CreateWebSocket(string path)
      {
         var ws = new WebSocket($"ws://{IPAddress.Loopback}:{Config.ComPort}" + path);
         ws.SetCredentials(Config.Username, Config.Password, false);
         //ws.SslConfiguration.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
         //{
         //   if (sslPolicyErrors != System.Net.Security.SslPolicyErrors.None)
         //   {
         //      Log.Error($"SslPolicyError: {sslPolicyErrors}");
         //      return false;
         //   }

         //   if (cert.GetPublicKeyString() != Config.ServerCertPublicKey)
         //   {
         //      Log.Error($"PublicKeyError: {cert.GetPublicKeyString()}");
         //      return false;
         //   }

         //   return true;
         //};

         RegisteredWebsockets.Add(ws);

         return ws;
      }

      public void Stop()
      {
         if (Stopped.Task.IsCompleted)
            return;

         lock (lockStop)
         {
            if (Stopped.Task.IsCompleted)
               return;

            Log.Info("Stopping");

            StarterPIDAliveChecker.Stop();

            Log.Info("Shutting down services");
            var shutdownTasks = new List<Task>();
            foreach(var ws in RegisteredWebsockets.Where(ws => new WebSocketState[] { WebSocketState.Closed, WebSocketState.Closing }.Contains(ws.ReadyState)))
            { 
               shutdownTasks.Add(Task.Run(() => ShutdownWS(ws)));
            }
            Task.WaitAll(shutdownTasks.ToArray());

            Log.Info("Shutdown services successful");

            Stopped.TrySetResult(true);

            Log.Info("Stopped");
         }
      }

      private async Task ShutdownWS(WebSocket ws)
      {
         using (var timeoutCancellationTokenSource = new CancellationTokenSource())
         {
            var shutdownTask = Task.Run(() =>
            {
               try
               {
                  ws.Close();
               }
               catch (Exception ex)
               {
                  Log.Error($"Failed to shutdown '{ws.Url}'", ex);
               }
            });

            if (await Task.WhenAny(Task.Delay(TimeSpan.FromSeconds(5), timeoutCancellationTokenSource.Token), shutdownTask) == shutdownTask)
            {
               timeoutCancellationTokenSource.Cancel();
               await shutdownTask;
            }
            else
               throw new TimeoutException("Failed to connect to shutdown in time");
         }
      }
   }
}
