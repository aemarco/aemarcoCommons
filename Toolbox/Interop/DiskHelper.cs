using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;

namespace aemarcoCommons.Toolbox.Interop
{
    public static class DiskHelper
    {
        #region Diskspace

        /// <summary>
        /// Calls Windows API to get Space Infos
        /// </summary>
        /// <param name="lpszPath">Must name a folder, must end with '\'.</param>
        /// <param name="lpFreeBytesAvailable"></param>
        /// <param name="lpTotalNumberOfBytes"></param>
        /// <param name="lpTotalNumberOfFreeBytes"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("Kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetDiskFreeSpaceEx(string lpszPath, ref long lpFreeBytesAvailable, ref long lpTotalNumberOfBytes, ref long lpTotalNumberOfFreeBytes);

        /// <summary>
        /// Windows API Call to determine free and total space in bytes
        /// </summary>
        /// <param name="folderName">folder to check space</param>
        /// <returns>free and total space in bytes</returns>
        public static (long free, long total) FreeAndTotalSpace(string folderName)
        {
            if (String.IsNullOrWhiteSpace(folderName)) throw new ArgumentException($"{nameof(folderName)} must be a valid path.", nameof(folderName));
            if (!folderName.EndsWith("\\")) folderName += "\\";

            long free = 0, total = 0, dummy = 0;
            return (GetDiskFreeSpaceEx(folderName, ref free, ref total, ref dummy))
                ? (free, total)
                : (-1, -1);
        }

        /// <summary>
        /// Windows API Call to determine free disc space in %
        /// </summary>
        /// <param name="folderName">folder to check space</param>
        /// <returns>% in range 0 to 100</returns>
        public static double FreeSpacePercentage(string folderName)
        {
            var (free, total) = FreeAndTotalSpace(folderName);
            return (free != -1)
                ? 1.0 * free / total * 100
                : -1;
        }

        /// <summary>
        /// Windows API Call to determine free space in bytes
        /// </summary>
        /// <param name="folderName">folder to check space</param>
        /// <returns>free space in bytes</returns>
        public static long FreeSpace(string folderName)
        {
            return FreeAndTotalSpace(folderName).free;
        }

        /// <summary>
        /// Windows API Call to determine total space in bytes
        /// </summary>
        /// <param name="folderName">folder to check space</param>
        /// <returns>total space in bytes</returns>
        public static long TotalSpace(string folderName)
        {
            return FreeAndTotalSpace(folderName).total;
        }

        #endregion
    }
}
