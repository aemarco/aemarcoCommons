using aemarcoCommons.Extensions.CryptoExtensions;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace aemarcoCommons.Extensions.FileExtensions;

public static class FileStuff
{


    /// <summary>
    /// Check if exclusive FileAccess is possible
    /// </summary>
    /// <param name="file">file to check</param>
    /// <returns>True if file exists and locked, false if the file does not exist or is available</returns>
    public static bool IsFileLocked(this FileInfo file)
    {
        file.Refresh();
        if (!file.Exists)
            return false;



        try
        {
            using var stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            return false;
        }
        catch (IOException ex)
        {
            int errorCode = ex.HResult & 0xFFFF;
            if (errorCode is 0x20 or 0x21)
            {
                return true;
            }

            // If it's another IOException (e.g., Disk Full, Device Not Ready), rethrow it
            // because waiting won't fix it.
            throw;


            //if (File.Exists(file.FullName))
            //{
            //    //the file is unavailable because it is:
            //    //still being written to
            //    //or being processed by another thread
            //    return true;
            //}
            //else
            //{
            //    //the file is unavailable because it is:
            //    //does not exist (has already been processed)
            //    return false;
            //}
        }
        catch (UnauthorizedAccessException)
        {
            // Optional: Handle permission errors (File is Read-Only or User has no rights)
            // If you only need to READ the file, change FileAccess.ReadWrite to FileAccess.Read
            return true;
        }

        //file is not locked
        //return false;
    }



    /// <summary>
    /// await till File can be exclusively accessed
    /// </summary>
    /// <param name="file">file to await for</param>
    /// <returns>true if file is accessible and exists</returns>
    public static async Task<bool> WaitForExclusiveAccess(this FileInfo file)
    {
        return await file.WaitForExclusiveAccess(CancellationToken.None);
    }

    /// <summary>
    /// await till File can be exclusively accessed
    /// </summary>
    /// <param name="file">file to await for</param>
    /// <param name="token">abort</param>
    /// <returns>true if file is accessible and exists</returns>
    public static async Task<bool> WaitForExclusiveAccess(this FileInfo file, CancellationToken token)
    {
        while (file.IsFileLocked())
        {
            await Task.Delay(1000, token);
        }

        file.Refresh();
        return File.Exists(file.FullName);
    }



    public static bool TryDelete(this FileInfo file)
    {
        if (!File.Exists(file.FullName)) return false;
        try
        {
            file.Delete();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static string TryBase64HashFromFile(this FileInfo file)
    {
        try
        {
            return file.Base64HashFromFile();
        }
        catch
        {
            return null;
        }
    }

    public static string Base64HashFromFile(this FileInfo file)
    {
        if (!File.Exists(file.FullName))
            throw new FileNotFoundException();

        using (Stream stream = File.OpenRead(file.FullName))
        {
            return stream.ToBase64HashString();
        }
    }

    public static Process OpenFile(this FileInfo file)
    {
        var result = Process.Start(
            new ProcessStartInfo(file.FullName)
            {
                UseShellExecute = true
            });
        return result;
    }
}