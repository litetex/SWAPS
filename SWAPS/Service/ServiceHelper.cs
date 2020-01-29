﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SWAPS.Service
{

   public static class ServiceHelper
   {
      [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
      private static extern bool ChangeServiceConfig(
          IntPtr hService,
          uint nServiceType,
          uint nStartType,
          uint nErrorControl,
          string lpBinaryPathName,
          string lpLoadOrderGroup,
          IntPtr lpdwTagId,
          [In] char[] lpDependencies,
          string lpServiceStartName,
          string lpPassword,
          string lpDisplayName);

      [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
      static extern IntPtr OpenService(
          IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

      [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
      private static extern IntPtr OpenSCManager(
          string machineName, string databaseName, uint dwAccess);

      [DllImport("advapi32.dll", EntryPoint = "CloseServiceHandle")]
      private static extern int CloseServiceHandle(IntPtr hSCObject);

      private const uint SERVICE_NO_CHANGE = 0xFFFFFFFF;
      private const uint SERVICE_QUERY_CONFIG = 0x00000001;
      private const uint SERVICE_CHANGE_CONFIG = 0x00000002;
      private const uint SC_MANAGER_ALL_ACCESS = 0x000F003F;

      [DllImport("kernel32.dll")]
      private static extern uint GetLastError();

      public static void ChangeStartMode(ServiceController svc, ServiceStartMode mode)
      {
         var scManagerHandle = OpenSCManager(null, null, SC_MANAGER_ALL_ACCESS);
         if (scManagerHandle == IntPtr.Zero)
         {
            throw new ExternalException($"Open Service Manager Error ({GetLastError()})");
         }

         var serviceHandle = OpenService(
             scManagerHandle,
             svc.ServiceName,
             SERVICE_QUERY_CONFIG | SERVICE_CHANGE_CONFIG);

         if (serviceHandle == IntPtr.Zero)
         {
            throw new ExternalException("Open Service Error");
         }

         var result = ChangeServiceConfig(
             serviceHandle,
             SERVICE_NO_CHANGE,
             (uint)mode,
             SERVICE_NO_CHANGE,
             null,
             null,
             IntPtr.Zero,
             null,
             null,
             null,
             null);

         if (!result)
         {
            int nError = Marshal.GetLastWin32Error();
            var win32Exception = new Win32Exception(nError);
            throw new ExternalException("Could not change service start type: "
                + win32Exception.Message);
         }

         CloseServiceHandle(serviceHandle);
         CloseServiceHandle(scManagerHandle);

      }
   }
}
