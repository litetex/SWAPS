using SWAPS.AdminCom;
using SWAPS.AdminCom.Service;
using SWAPS.CMD;
using SWAPS.Config;
using SWAPS.Lockfile;
using SWAPS.Shared.Com.IPC.Payload;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace SWAPS
{
   public class Runner
   {
      protected LockFileManager LockFileManager { get; set; }

      protected AdminCommunictator AdminCommunictator { get; set; }

      protected Configuration Config { get; set; }

      protected RunCmdOptions CmdOptions { get; set; }

      public Runner(Configuration configuration, RunCmdOptions cmdOptions)
      {
         Config = configuration;
         CmdOptions = cmdOptions;
      }

      public void Run()
      {
         if (!string.IsNullOrWhiteSpace(Config.Name))
            Console.Title = Config.Name;

         using (LockFileManager = new LockFileManager(Config.LockFileConfig, Config.Config.SavePath)
         {
            LockFileFoundMode = CmdOptions.LockFileFoundMode
         })
         {
            try
            {
               try
               {
                  LockFileManager.Init();
               }
               catch(LockFileAbortException lfaex)
               {
                  Log.Error("Aborting due to lockfile", lfaex);
                  Environment.ExitCode = -1;
                  return;
               }
               

               AdminCommunictator = new AdminCommunictator(
                  CmdOptions.LogToFile,
                  CmdOptions.LogFileRetainCount,
                  CmdOptions.Verbose,
                  CmdOptions.ShowServerConsole,
                  CmdOptions.UseUnencryptedCom);

               AdminCommunictator.Start();

               Log.Info("Starting services");
               foreach (var serviceConfig in Config.ServiceConfigs)
               {
                  Log.Info($"Starting service '{serviceConfig.ServiceName}'");
                  AdminCommunictator.StartServiceManager
                    .Invoke(
                       new ServiceStart()
                       {
                          Name = serviceConfig.ServiceName,
                          Timeout = Config.ServiceStartTimeout
                       },
                       Config.ServiceStartTimeout.Add(TimeSpan.FromSeconds(10)))
                    .Wait();
               }

               Log.Info($"Waiting {Config.ServiceProperlyStartedDelay} for the service to become fully operational");
               Thread.Sleep(Config.ServiceProperlyStartedDelay);

               StartProcesses();

               Log.Info("Stopping services");
               foreach (var serviceConfig in Config.ServiceConfigs)
               {
                  Log.Info($"Stopping service '{serviceConfig.ServiceName}'");
                  AdminCommunictator.StopServiceManager
                     .Invoke(
                        new ServiceStop()
                        {
                           Name = serviceConfig.ServiceName,
                           CrashOnServiceNotFound = Config.CrashOnUpdateServiceNotFound ?? serviceConfig.CrashOnUpdateServiceNotFound,
                        },
                        TimeSpan.FromSeconds(10))
                     .Wait();
               }

               Thread.Sleep(Config.StayingOpenBeforeEnding);

            }
            catch (Exception e)
            {
               Log.Error(e);
            }
            finally
            {
               AdminCommunictator?.Stop();
            }
         }
      }

      private void StartProcesses()
      {
         Log.Info("Starting processes");

         var lockKeyTasks = new object();
         var keyTasks = new Dictionary<string, Task>();

         var tasks = Config.ProcessConfigs.Select(processConfig =>
         {

            var task = Task.Run(() =>
            {
               try
               {
                  RunProcess(processConfig, keyTasks);
               }
               catch (Exception ex)
               {
                  Log.Error("Failed to start process", ex);
               }
            });

            if (!string.IsNullOrWhiteSpace(processConfig.Key))
               lock (lockKeyTasks)
                  keyTasks.Add(processConfig.Key, task);

            return task;
         });

         Log.Info("Waiting for processes to finish...");
         Task.WhenAll(tasks).Wait();

         Log.Info("All processes finished");
      }

      private void RunProcess(ProcessConfig processConfig, Dictionary<string, Task> keyTasks)
      {
         var identifier = !string.IsNullOrWhiteSpace(processConfig.Key) ? processConfig.Key : processConfig.FilePath;

         if (processConfig.DependsOn != null)
         {
            foreach (var dependingKey in processConfig.DependsOn)
            {
               if (keyTasks.ContainsKey(dependingKey))
               {
                  Log.Info($"[{identifier}] Waiting for '{dependingKey}'");
                  keyTasks[dependingKey].Wait();
               }
            }
         }

         Log.Info($"[{identifier}] Starting '{processConfig.FilePath}' in '{processConfig.WorkDir}' with Timeout='{processConfig.Timeout}' {(processConfig.Async ? "Async" : "")}");
         var process = new Process
         {
            StartInfo = new ProcessStartInfo
            {
               WorkingDirectory = processConfig.WorkDir,
               FileName = processConfig.FilePath,
               Arguments = processConfig.Args,
            }
         };
         process.Start();

         if (processConfig.Async)
            return;

         if (processConfig.Timeout == null)
            process.WaitForExit();
         else if (!process.WaitForExit((int)processConfig.Timeout.Value.TotalMilliseconds))
         {
            Log.Info($"[{identifier}] Process timed out, killing it");
            process.Kill();
         }
      }

   }
}
