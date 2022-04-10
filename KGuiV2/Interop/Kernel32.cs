using System.Runtime.InteropServices;
using System;

namespace KGuiV2.Interop
{
    internal static class Kernel32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct MemoryStatusEx
        {
            public uint  Length = Convert.ToUInt32(Marshal.SizeOf<MemoryStatusEx>());
            public uint  MemoryLoad;
            public ulong TotalPhys;
            public ulong AvailPhys;
            public ulong TotalPageFile;
            public ulong AvailPageFile;
            public ulong TotalVirtual;
            public ulong AvailVirtual;
            public ulong AvailExtendedVirtual;
        }

        /// <summary>
        /// <br>Loads the specified module into the address space of the calling process</br>
        /// <br>The specified module may cause other modules to be loaded.</br>
        /// </summary>
        /// <param name="lpLibFileName">
        /// <br>The name of the module.</br>
        /// <br>This can be either a library module (a .dll file) or an executable module (an .exe file).</br>
        /// </param>
        /// <returns>
        /// <br>If the function succeeds, the return value is a handle to the module.</br>
        /// <br>If the function fails, the return value is <see cref="IntPtr.Zero"/>. To get extended error information, call GetLastError.</br>
        /// </returns>
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern nint LoadLibrary(
            string lpLibFileName
        );

        /// <summary>
        /// Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
        /// </summary>
        /// <param name="hLibModule">A handle to the loaded library module.</param>
        /// <returns>
        /// <br>If the function succeeds, the return value is true.</br>
        /// <br>If the function fails, the return value is false. To get extended error information, call the GetLastError function.</br>
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(
            nint hLibModule
        );

        /// <summary>
        /// Retrieves information about the system's current usage of both physical and virtual memory.
        /// </summary>
        /// <param name="lpBuffer">The <see cref="MemoryStatusEx"/> structure that receives information about current memory availability.</param>
        /// <returns>
        /// <br>If the function succeeds, the return value is true.</br>
        /// <br>If the function fails, the return value is false. To get extended error information, call GetLastError.</br>
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalMemoryStatusEx(
            [In, Out] ref MemoryStatusEx lpBuffer
        );
    }
}
