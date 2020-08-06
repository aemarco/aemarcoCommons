using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Extensions.netExtensions
{
    public static class FileInfoExtensions
    {


        /// <summary>
        /// Check if exclusive FileAccess is possible
        /// </summary>
        /// <param name="file">file to check</param>
        /// <returns>True if file exists and locked, false if the file does not exist or is available</returns>
        public static bool IsFileLocked(this FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {

                if (File.Exists(file.FullName))
                {
                    //the file is unavailable because it is:
                    //still being written to
                    //or being processed by another thread
                    return true;
                }
                else
                {
                    //the file is unavailable because it is:
                    //does not exist (has already been processed)
                    return false;
                }
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
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

            using Stream stream = File.OpenRead(file.FullName);
            return stream.ToBase64HashString();
        }


    }
}
