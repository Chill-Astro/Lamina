using System;
using System.Runtime.InteropServices;
using Microsoft.UI.Dispatching; // Ensure this is the correct DispatcherQueue to use

public class WindowsSystemDispatcherQueueHelper
{
    private object dispatcherQueueController = null;

    [DllImport("CoreMessaging.dll")]
    private static extern int CreateDispatcherQueueController(
        [In] DispatcherQueueOptions options,
        [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object dispatcherQueueController);

    [StructLayout(LayoutKind.Sequential)]
    private struct DispatcherQueueOptions
    {
        public int Size;
        public int ThreadType;
        public int ApartmentType;
    }

    public void EnsureWindowsSystemDispatcherQueueController()
    {
        if (DispatcherQueue.GetForCurrentThread() != null)
        {
            return; // A DispatcherQueue already exists, so no need to create a new one.
        }

        if (dispatcherQueueController == null)
        {
            DispatcherQueueOptions options = new()
            {
                Size = Marshal.SizeOf(typeof(DispatcherQueueOptions)),
                ThreadType = 2,  // DQTYPE_THREAD_CURRENT
                ApartmentType = 2 // DQTAT_COM_STA
            };

            CreateDispatcherQueueController(options, ref dispatcherQueueController);

            // Prevent premature garbage collection
            GC.KeepAlive(dispatcherQueueController);
        }
    }
}
