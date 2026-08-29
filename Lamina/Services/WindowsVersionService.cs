using System.Runtime.InteropServices;

namespace Lamina.Services;

public static class WindowsVersionService
{
    [DllImport("kernel32.dll")]
    private static extern bool GetVersionEx(ref OSVERSIONINFOEX osvi);

    [StructLayout(LayoutKind.Sequential)]
    private struct OSVERSIONINFOEX
    {
        public int dwOSVersionInfoSize;
        public int dwMajorVersion;
        public int dwMinorVersion;
        public int dwBuildNumber;
        public int dwPlatformId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szCSDVersion;
    }

    public static bool IsWindows10()
    {
        try
        {
            var osvi = new OSVERSIONINFOEX();
            osvi.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));
            
            if (GetVersionEx(ref osvi))
            {
                // Windows 10 is version 10.0, Windows 11 is version 10.0 with build number >= 22000
                return osvi.dwMajorVersion == 10 && osvi.dwMinorVersion == 0 && osvi.dwBuildNumber < 22000;
            }
        }
        catch
        {
            // Fallback: assume Windows 10 if detection fails
            return true;
        }
        
        return false;
    }

    public static bool IsWindows11()
    {
        return !IsWindows10();
    }
}
