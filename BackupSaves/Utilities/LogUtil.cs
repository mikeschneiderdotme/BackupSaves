using System;
using System.IO;
using System.Threading;

namespace BackupSaves.Utilities
{
    public static class LogUtil
    {
        /// <summary>
        /// A write lock object to handle system I/O exceptions with the log file.
        /// </summary>
        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Writes a string to a log.txt file in the backup directory.
        /// </summary>
        /// <param name="log">Log message being written to the log.txt file.</param>
        public static void WriteToLog(string log)
        {
            // Set status to locked.
            _readWriteLock.EnterWriteLock();

            try
            {
                // Append log to file.
                using (var file = new StreamWriter(FileUtil.GetFriendlyLogFileName(), true))
                {
                    file.Write(string.Format("[{0}]: {1}\n", DateTime.Now, log));
                    file.Close();
                }
            }
            finally
            {
                // Release lock.
                _readWriteLock.ExitWriteLock();
            }
        }
    }
}
