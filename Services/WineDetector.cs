using System;
using System.Runtime.InteropServices;

namespace SwiftXP.SPT.Common.Services;

public static class WineDetector
{
    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    public static bool IsRunningOnWine()
    {
        try
        {
            IntPtr ntdllModule = GetModuleHandle("ntdll.dll");

            if (ntdllModule == IntPtr.Zero)
            {
                return false;
            }

            IntPtr wineVersionProc = GetProcAddress(ntdllModule, "wine_get_version");
            return wineVersionProc != IntPtr.Zero;
        }
        catch
        {
            return false;
        }
    }
}