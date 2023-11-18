using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Timers;
using SWAPS.Config;
using SWAPS.Util;
using SWAPS.Persistence;

namespace SWAPS.Lockfile
{
   public class LockFileManager : IDisposable
   {
      public LockFile LockFile { get; private set; } = new LockFile();

      protected LockFileConfig LockFileConfig { get; set; }

      protected string LockFileSavePath { get; set; }

      protected PersistenceManager<LockFile> LockFilePersister = new PersistenceManager<LockFile>();

      public LockFileFoundMode LockFileFoundMode { get; set; } = LockFileFoundMode.Terminate;

      protected Timer LockFileUpdateTimer { get; set; }

      protected bool ValidLockFileAlreadyExists { get; set; } = false;

      private bool disposedValue;

      public LockFileManager(LockFileConfig config, string configurationFilePath)
      {
         LockFileConfig = config;
         LockFileSavePath = Path.Combine(configurationFilePath + LockFileConfig.LockFileExtension ?? ".lock");
      }

      /// <summary>
      /// 
      /// </summary>
      /// <exception cref="LockFileAbortException">When aborting should be done, becuase of a valid lockfile</exception>
      public void Init()
      {
         Log.Info($"LockFile-subsystem is {(LockFileConfig.Enabled ? "en" : "dis")}abled");
         if (!LockFileConfig.Enabled)
            return;

         ValidLockFileAlreadyExists = CheckLockFile();
         if (ValidLockFileAlreadyExists)
         {
            Log.Warn($"Found a valid lockfile '{LockFileSavePath}'");
            LockFileFound();
         }

         UpdateLockFile(true);

         LaunchLockFileUpdater();
      }

      /// <summary>
      /// 
      /// </summary>
      /// <returns><code>true</code>when a valid lockfile was found</returns>
      protected bool CheckLockFile()
      {
         Log.Info($"Checking for lockfile at '{LockFileSavePath}'");

         if (!File.Exists(LockFileSavePath))
         {
            Log.Info("Found no lockfile");
            return false;
         }

         Log.Info($"Found lockfile; Loading from '{LockFileSavePath}'");
         try
         {
            LockFilePersister.Load(LockFile, LockFileSavePath);
            Log.Info("Loaded lockfile");
         }
         catch (Exception ex)
         {
            Log.Error("Failed to load lockfile", ex);
            return false;
         }

         if (LockFile.FileVersion != LockFile.CURRENT_VERSION)
         {
            Log.Warn("Found an not matching lockfile version");
         }

         Log.Info("Running lockfile-validation");
         var valid = ValidateLockFile();
         Log.Info($"Lockfilevalidation returned {valid}");

         if (!valid)
         {
            DeleteLockFile();
            return false;
         }

         return true;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <returns><code>true</code> if the lockfile is valid, other wise <code>false</code></returns>
      protected bool ValidateLockFile()
      {
         if (LockFile.PID < 0)
         {
            Log.Warn($"Lockfile contains invalid PID='{LockFile.PID}'");
            return false;
         }

         if (!ProcessAliveChecker.CheckIfPIDAlive(LockFile.PID))
         {
            Log.Warn($"PID of lockfile '{LockFile.PID}' is not alive");
            return false;
         }

         if ((DateTime.Now - LockFile.UpdateDate) >= LockFileConfig.LockFileInvalidAfter)
         {
            Log.Warn($"Lockfile was updated by a process at {LockFile.UpdateDate} which is outside the range of +/-3d of the current time");
            return false;
         }

         return true;
      }

      /// <summary>
      /// 
      /// </summary>
      /// <exception cref="LockFileAbortException">When aborting should be done, becuase of a valid lockfile</exception>
      protected void LockFileFound()
      {
         if (LockFileFoundMode == LockFileFoundMode.AskUser)
         {
            ConsoleController.TryShowConsole();
            Console.WriteLine("************** DETECTED VALID LOCKFILE **************");
            Console.WriteLine($"Another instance of the program is running and using the current configuration (which has the lockfile-check enabled)");
            Console.WriteLine();
            Console.WriteLine("Informations about the locking process:");

            Process lockingProcess = null;
            try
            {
               lockingProcess = Process.GetProcessById(LockFile.PID);
            }
            catch
            {
               // Skip
            }

            var keyVal = new Dictionary<string, string>
            {
               { "PID", LockFile.PID.ToString() },
               { "Name", lockingProcess?.ProcessName },
               { "Location", lockingProcess?.MainModule?.FileName },
               { "AppVersion", LockFile.AppVersion },
               { "Started at", LockFile.CreationDate.ToString("yyyy-MM-dd HH:mm:ss,fff") },
               { "Updated at", LockFile.UpdateDate.ToString("yyyy-MM-dd HH:mm:ss,fff") },
               { "LockFile-Version", LockFile.FileVersion.ToString() }
            };


            var paddKeyCount = keyVal.Keys.Max(k => k.Length);
            foreach (var kv in keyVal)
               Console.WriteLine($"\t - {kv.Key.PadRight(paddKeyCount)}: {kv.Value ?? "?"}");

            Console.WriteLine();
            while (true)
            {
               Console.WriteLine("Ignore and start anyway (not recommended)? [y/n]");
               var ans = Console.ReadLine();
               if (ans == null)
                  continue;

               var input = ans.ToLower().Trim();
               if ("y".Equals(input))
               {
                  LockFileFoundMode = LockFileFoundMode.Ignore;
                  break;
               }
               else if ("n".Equals(input))
               {
                  LockFileFoundMode = LockFileFoundMode.Terminate;
                  break;
               }
            }
         }

         if (LockFileFoundMode == LockFileFoundMode.Ignore)
         {
            Log.Info($"Ignoring lockfile");
            return;
         }

         throw new LockFileAbortException($"Found valid lockfile '{LockFileSavePath}'");
      }

      public void DeleteLockFile()
      {
         if (File.Exists(LockFileSavePath))
         {
            Log.Info($"Deleting lockfile '{LockFileSavePath}'");
            File.Delete(LockFileSavePath);
            Log.Info("Deleting lockfile");
         }
      }

      protected void UpdateLockFile(bool firstUpdate = false)
      {
         if (ValidLockFileAlreadyExists)
            return;

         Log.Info($"Updating lockfile '{LockFileSavePath}'");

         try
         {
            LockFile.FileVersion = LockFile.CURRENT_VERSION;

            if (firstUpdate)
               LockFile.CreationDate = DateTime.Now;
            LockFile.UpdateDate = DateTime.Now;

            LockFile.PID = Process.GetCurrentProcess().Id;
            LockFile.AppVersion = Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString() ?? "-";

            LockFilePersister.Save(LockFile, LockFileSavePath);
         }
         catch (Exception ex)
         {
            Log.Error("Failed to update lockfile", ex);
         }
      }

      protected void LaunchLockFileUpdater()
      {
         if (ValidLockFileAlreadyExists || LockFileUpdateTimer != null)
            return;

         Log.Info($"Starting {nameof(LockFileUpdateTimer)}");
         LockFileUpdateTimer = new Timer(LockFileConfig.ReNewLockFileAfter.TotalMilliseconds)
         {
            AutoReset = true,
            Enabled = true,
         };

         Log.Info($"Registering {nameof(LockFileUpdateTimer)} shutdown hook to process");
         AppDomain.CurrentDomain.ProcessExit += (s, ev) =>
         {
            LockFileUpdateTimer?.Stop();
            LockFileUpdateTimer = null;
         };
         LockFileUpdateTimer.Elapsed += (s, ev) =>
         {
            UpdateLockFile();
         };

         Log.Info($"Started {nameof(LockFileUpdateTimer)}");
      }

      protected void CleanUp()
      {
         if (!LockFileConfig.Enabled || ValidLockFileAlreadyExists)
            return;

         try
         {
            DeleteLockFile();
         }
         catch (Exception ex)
         {
            Log.Error("Unable to delete lockfile", ex);
         }

         LockFileUpdateTimer?.Stop();
         LockFileUpdateTimer = null;
      }

      protected virtual void Dispose(bool disposing)
      {
         if (!disposedValue)
         {
            if (disposing)
            {
               CleanUp();
            }

            disposedValue = true;
         }
      }

      public void Dispose()
      {
         // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
         Dispose(disposing: true);
         GC.SuppressFinalize(this);
      }
   }
}
