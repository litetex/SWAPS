using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Onova;
using Onova.Models;
using Onova.Services;
using SWAPS.Util;

namespace SWAPS.Update
{
   public class Updater : IDisposable
   {
      public const UpdateMode DEFAULT_MODE = UpdateMode.Notify;

      private bool disposedValue;

      private CancellationTokenSource DisposeCTS { get; } = new CancellationTokenSource();

      protected readonly object _lockSearchAndPrepareUpdate = new object();


      private IUpdateManager UpdateManager { get; set; }


      private UpdateMode UpdateMode { get; set; }

      private bool ForceUpdate { get; set; }

      private Version CurrentVersion { get; set; }
      private Version UpdateVersionTarget { get; set; }

      private bool PreparedUpdate { get; set; }

      public Updater(UpdateMode updateMode = UpdateMode.Always)
      {
         UpdateMode = updateMode;

         var os = "win";
         var arch = Environment.Is64BitOperatingSystem ? "64" : "86";

         var assemblyMetadata =
            AssemblyMetadata.FromAssembly(
                  Assembly.GetEntryAssembly(),
                  System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);

         CurrentVersion = assemblyMetadata.Version;

         UpdateManager =
            new UpdateManager(
               assemblyMetadata,
               new GithubPackageResolver("litetex", "SWAPS", $"SWAPS-{os}-x{arch}*"),
               new ZipPackageExtractor()
         );
      }

      public void ForceUpdateNow()
      {
         ForceUpdate = true;

         SearchAndPrepareUpdate();
         FinalizeUpdate();
      }

      public void OnStart()
      {
         if (UpdateMode == UpdateMode.None)
            return;

         Task.Run(() =>
         {
            lock (_lockSearchAndPrepareUpdate)
               SearchAndPrepareUpdate();
         });
      }

      protected void SearchAndPrepareUpdate()
      {
         if (UpdateVersionTarget != null)
            return;

         if(CurrentVersion.Major == 0 && CurrentVersion.Minor == 0 && CurrentVersion.Build == 0)
         {
            Log.Warn($"Version '{CurrentVersion}' is invalid! Looks like no version was set while building this executable");
            return;
         }

         try
         {
            Log.Info($"Searching for updates; Current version is '{CurrentVersion}'");
            var verRes = UpdateManager.CheckForUpdatesAsync(DisposeCTS.Token).Result;
            if (!verRes.CanUpdate)
            {
               Log.Info($"No update was found; Latest version is '{verRes?.LastVersion}'");
               return;
            }

            if (UpdateMode == UpdateMode.Notice || UpdateMode == UpdateMode.Notify)
            {
               Log.Info($"↑↑↑ Found update '{verRes.LastVersion}' ↑↑↑\r\n" +
                  $"+++++++++++++++++++++++++++++++++++\r\n" +
                  $"+ Run with --update to install it +\r\n" +
                  $"+++++++++++++++++++++++++++++++++++");

               if (UpdateMode == UpdateMode.Notify)
               {
                  ConsoleController.TryShowConsole();
               }

               return;
            }

            UpdateVersionTarget = verRes.LastVersion;
            Log.Info($"Found update to '{UpdateVersionTarget}', preparing it");
            using (var progessBar = ForceUpdate ? new ConsoleProgressBar() : null)
               UpdateManager.PrepareUpdateAsync(UpdateVersionTarget, progessBar, DisposeCTS.Token).Wait();


            PreparedUpdate = true;
            Log.Info($"Update '{UpdateVersionTarget}' is prepared");
         }
         catch (Exception ex)
         {
            UpdateVersionTarget = null;
            PreparedUpdate = false;
            Log.Error($"Failed to execute", ex);
         }
      }

      public void OnEnd()
      {
         if (UpdateMode == UpdateMode.None)
            return;

         FinalizeUpdate();
      }

      protected void FinalizeUpdate()
      {
         if (!PreparedUpdate)
            return;

         try
         {
            Log.Info("Launching updater; Update should be installed after exit");
            UpdateManager.LaunchUpdater(UpdateVersionTarget, false);
         }
         catch (Exception ex)
         {
            UpdateVersionTarget = null;
            Log.Error($"Failed to execute", ex);
         }
      }

      protected virtual void Dispose(bool disposing)
      {
         if (!disposedValue)
         {
            if (disposing)
            {
               // dispose managed state (managed objects)
               DisposeCTS.Cancel();
               UpdateManager.Dispose();
            }

            // free unmanaged resources (unmanaged objects) and override finalizer
            // set large fields to null
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
