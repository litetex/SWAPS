using JKang.IpcServiceFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SWAPS.Admin.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using SWAPS.Shared.Admin;
using JKang.IpcServiceFramework.Tcp;
using SWAPS.Admin.Services;
using System.Threading;
using SWAPS.Shared.Admin.Services;
using System.Linq;
using SWAPS.Shared;

namespace SWAPS.Admin.Communication
{
   public class Communicator
   {
      private ComConfig Config { get; set; }

      #region Tasks
      private ProcessAliveChecker StarterPIDAliveChecker { get; set; }

      private HandshakeWithinTimeout HandshakeWithinTimeout { get; set; }
      #endregion Tasks

      public List<ComServiceHost> ServiceHosts { get; set; } = new List<ComServiceHost>();

      private IIpcServiceHost ControllerServiceHost { get; set; }

      private TaskCompletionSource<bool> Stopped { get; set; }

      private readonly object lockStop = new object();

      public Communicator(ComConfig configuration)
      {
         Contract.Requires(configuration != null);
         Config = configuration;
      }

      public void Run()
      {
         Start();
         StartAndWaitForStop();
      }

      protected void Start()
      {
         Stopped = new TaskCompletionSource<bool>();

         StarterPIDAliveChecker = new ProcessAliveChecker(Config.StarterPID, TimeSpan.FromSeconds(2), () => Stop());

         Log.Info($"Doing inital check if starter process[PID={Config.StarterPID}] is alive");
         if (!ProcessAliveChecker.CheckIfStarterPIDAlive(Config.StarterPID))
            throw new ArgumentException($"StarterPID={Config.StarterPID} not found!");

         Log.Info($"Starting {nameof(StarterPIDAliveChecker)}; StarterPID={Config.StarterPID}");
         StarterPIDAliveChecker.Start();

         Log.Info($"Starting {nameof(HandshakeWithinTimeout)}-Task; Timeout={Config.StarterTimeout}");
         HandshakeWithinTimeout = new HandshakeWithinTimeout();
         HandshakeWithinTimeout.StartTimeout(Config.StarterTimeout, () => Stop());


         ControllerServiceHost = GetBaseIpcServiceHostBuilder()
             .AddTcpEndpoint<IAdminControllerService>(
                  name: "ComEndpoint",
                  ipEndpoint: IPAddress.Loopback,
                  port: Config.ComTCPPort)
             .Build();
      }

      protected IpcServiceHostBuilder GetBaseIpcServiceHostBuilder()
      {
         return new IpcServiceHostBuilder(ConfigureServices(new ServiceCollection()).BuildServiceProvider());
      }

      private IServiceCollection ConfigureServices(IServiceCollection services)
      {
         return services
             .AddLogging(builder =>
             {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
             })
             .AddIpc(builder =>
             {
                builder
                  .AddTcp()
                  .AddService<IAdminControllerService, AdminControllerService>()
                  .AddService<IServiceControllerService, ServiceControllerService>();
             });
      }

      protected void StartAndWaitForStop()
      {
         var source = new CancellationTokenSource();

         Log.Info("Waiting for stop");

         Task.WhenAny(ControllerServiceHost.RunAsync(source.Token), Stopped.Task).Wait();
         source.Cancel();

         Log.Info("Stop occured!");
      }

      public void Handshake()
      {
         Log.Info("Handshake");
         HandshakeWithinTimeout?.Handshake();
      }

      public int CreateNewServiceHostWithService<T>() where T : class
      {
         Type t = typeof(T);

         Log.Info($"Start new ServieHost-Request: {t}");

         string name = $"{t.AssemblyQualifiedName}-{DateTime.UtcNow:yyyyMMdd-HHmmssfff}";
         while(ServiceHosts.Exists(sh => sh.Name.Equals(name)))
         {
            name = $"{t.AssemblyQualifiedName}-{DateTime.UtcNow:yyyyMMdd-HHmmssfff}";
            Thread.Sleep(10);
         }

         int port = NetworkUtil.GetFreeTcpPort();

         var host = GetBaseIpcServiceHostBuilder()
            .AddTcpEndpoint<T>(
               name: name,
               ipEndpoint: IPAddress.Loopback,
               port: port)
            .Build();


         var cancelTokenSource = new CancellationTokenSource();
         host.RunAsync(cancelTokenSource.Token);
         Log.Info($"Started new ServieHost[Port={port},Name='{name}']");

         ServiceHosts.Add(new ComServiceHost()
         {
            ComPort = port,
            Host = host,
            Name = name,
            Service = t,
            CancelTokenSource = cancelTokenSource,
         });

         return port;
      }

      public void StopServiceHost(int port)
      {
         Log.Info($"Trying to shutdown ServiceHost on Port={port}");
         var servicehost = ServiceHosts.Find(x => x.ComPort == port);
         if (servicehost == null)
            return;

         ShutdownAndRemove(servicehost);
      }

      protected void ShutdownAndRemove(ComServiceHost serviceHost)
      {
         Log.Info($"Invoking Cancel for ServiceHost[Port={serviceHost.ComPort},Name='{serviceHost.Name}']");
         serviceHost.CancelTokenSource.Cancel();

         var success = ServiceHosts.Remove(serviceHost);
         Log.Info($"Cancelled and removed{(!success ? " (not in List!)" : "")} ServiceHost[Port={serviceHost.ComPort},Name='{serviceHost.Name}']");
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

            ServiceHosts.ForEach(sh => ShutdownAndRemove(sh));

            StarterPIDAliveChecker.Stop();

            Stopped.TrySetResult(true);
         }
      }
   }
}
