using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace WinTools.WindTools
{
    public class DiskSpace
    {

        /// <summary>
        /// Windows API Call to determine free disc space in %
        /// </summary>
        /// <param name="folderName">folder to check space</param>
        /// <returns>% in range 0 to 100</returns>
        public static double FreeSpacePercentage(string folderName)
        {
            var (free, total) = FreeAndTotalSpace(folderName);
            if (free == -1) return -1;
            else return 1.0 * free / total * 100;
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

        /// <summary>
        /// Windows API Call to determine free and total space in bytes
        /// </summary>
        /// <param name="folderName">folder to check space</param>
        /// <returns>free and total space in bytes</returns>
        public static (long free, long total) FreeAndTotalSpace(string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException("folderName");
            }

            if (!folderName.EndsWith("\\"))
            {
                folderName += '\\';
            }

            long free = 0, total = 0, dummy = 0;

            if (GetDiskFreeSpaceEx(folderName, ref free, ref total, ref dummy))
            {
                return (free, total);
            }
            else
            {
                return (-1, -1);
            }
        }


        #region winApi

        [SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("Kernel32", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool GetDiskFreeSpaceEx
        (
            string lpszPath,                    // Must name a folder, must end with '\'.
            ref long lpFreeBytesAvailable,
            ref long lpTotalNumberOfBytes,
            ref long lpTotalNumberOfFreeBytes
        );

        #endregion

    }
}
