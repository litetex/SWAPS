using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SWAPS.Util
{
   public static class ConsoleController
   {
      /// <summary>
      /// NOTE: This does not normally not work in Visual Studio with the developer console
      /// </summary>
      public static void TryHideConsole()
      {
         try
         {
            ChangeConsoleMode(ShowWindowMode.SW_MINIMIZE);
         }
         catch (Exception ex)
         {
            Log.Warn("Failed to hide console", ex);
         }
      }

      /// <summary>
      /// NOTE: This does not normally not work in Visual Studio with the developer console
      /// </summary>
      public static void TryShowConsole()
      {
         try
         {
            ChangeConsoleMode(ShowWindowMode.SW_RESTORE);
         }
         catch (Exception ex)
         {
            Log.Warn("Failed to hide console", ex);
         }
      }

      /// <summary>
      /// NOTE: This does not normally not work in Visual Studio with the developer console
      /// </summary>
      /// <param name="showWindowMode"></param>
      public static void ChangeConsoleMode(ShowWindowMode showWindowMode)
      {
         ShowWindow(Process.GetCurrentProcess().MainWindowHandle, (int)showWindowMode);
      }

      [DllImport("user32.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
      [return: MarshalAs(UnmanagedType.Bool)]
      private static extern bool ShowWindow([In] IntPtr hWnd, [In] int nCmdShow);

      /// <summary>
      /// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow
      /// </summary>
      public enum ShowWindowMode
      {
         /// <summary>
         /// Hides the window and activates another window. 
         /// </summary>
         SW_HIDE = 0,
         /// <summary>
         /// Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time. 
         /// </summary>
         SW_SHOWNORMAL = 1,
         /// <summary>
         /// Activates the window and displays it as a minimized window. 
         /// </summary>
         SW_SHOWMINIMIZED = 2,
         /// <summary>
         /// Maximizes the specified window. 
         /// </summary>
         SW_MAXIMIZE = 3,
         /// <summary>
         /// Activates the window and displays it as a maximized window. 
         /// </summary>
         SW_SHOWMAXIMIZED = 3,
         /// <summary>
         /// Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except that the window is not activated. 
         /// </summary>
         SW_SHOWNOACTIVATE = 4,
         /// <summary>
         /// Activates the window and displays it in its current size and position. 
         /// </summary>
         SW_SHOW = 5,
         /// <summary>
         /// Minimizes the specified window and activates the next top-level window in the Z order. 
         /// </summary>
         SW_MINIMIZE = 6,
         /// <summary>
         /// Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated. 
         /// </summary>
         SW_SHOWMINNOACTIVE = 7,
         /// <summary>
         /// Displays the window in its current size and position. This value is similar to SW_SHOW, except that the window is not activated. 
         /// </summary>
         SW_SHOWNA = 8,
         /// <summary>
         /// Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window. 
         /// </summary>
         SW_RESTORE = 9,
         /// <summary>
         /// Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application. 
         /// </summary>
         SW_SHOWDEFAULT = 10,
         /// <summary>
         /// Minimizes a window, even if the thread that owns the window is not responding. This flag should only be used when minimizing windows from a different thread. 
         /// </summary>
         SW_FORCEMINIMIZE = 11,
      }
   }
}
