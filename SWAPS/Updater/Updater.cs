using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Onova;
using Onova.Models;
using Onova.Services;

namespace SWAPS.Updater
{
   public class Updater : IDisposable
   {
      private IUpdateManager UpdateManager { get; set; }

      private bool disposedValue;

      public Updater()
      {
         var os = "win";
         var arch = Environment.Is64BitOperatingSystem ? "64" : "32";

         UpdateManager =
            new UpdateManager(
               AssemblyMetadata.FromAssembly(
                 Assembly.GetEntryAssembly(),
                 System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName),
                  new GithubPackageResolver("litetex", "SWAPS", $"SWAPS-{os}-x{arch}"),
                  new ZipPackageExtractor()
        );
      }

      protected virtual void Dispose(bool disposing)
      {
         if (!disposedValue)
         {
            if (disposing)
            {
               // dispose managed state (managed objects)
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
