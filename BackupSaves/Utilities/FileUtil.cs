using System;
using System.IO;

namespace BackupSaves.Utilities
{
    public static class FileUtil
    {
        /// <summary>
        /// Constant that contains the backup directory path.
        /// </summary>
        public const string BASE_BACKUP_DIRECTORY = @"D:\SavesBackup\";

        /// <summary>
        /// Constant that contains the backup directory path.
        /// </summary>
        public const string GAME_BACKUP_DIRECTORY = BASE_BACKUP_DIRECTORY + @"\GameSaves\";

        /// <summary>
        /// Constant that contains the log file path.
        /// </summary>
        public const string LOG_PATH = BASE_BACKUP_DIRECTORY + @"\Logs\";

        /// <summary>
        /// File path for the steam remote saves directory.
        /// </summary>
        public const string STEAM_LOCATION = @"C:\Program Files (x86)\Steam\userdata\62462215\";

        /// <summary>
        /// File path for the uplay saves directory.
        /// </summary>
        public const string UPLAY_LOCATION = @"C:\Program Files (x86)\Ubisoft\Ubisoft Game Launcher\savegames\7e7ad2e9-f90b-494e-a1cc-c4ec784c9b3c\";

        /// <summary>
        /// Check to see that the backup directory exists; Creates the directory if it doesn't
        /// </summary>
        /// <param name="destDirName">Destination directory path.</param>
        public static void DestinationCheck(string destDirName, bool cleanDestDir)
        {
            DirectoryInfo dir = new DirectoryInfo(destDirName);

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
                LogUtil.WriteToLog("Destination Directory created at " + destDirName);
            }
            else if (cleanDestDir)
            {
                UpdateDirectory(destDirName);
            }
        }

        public static void InitiateLogFile()
        {
            DestinationCheck(LOG_PATH, false);
            string logFileName = GetFriendlyLogFileName();

            using (FileStream stream = new FileStream(logFileName, FileMode.Create))
            {
                if (!File.Exists(logFileName))
                {
                    stream.WriteByte(new byte());
                    stream.Flush();
                }
                stream.Close();
            }
        }

        public static string GetFriendlyLogFileName()
        {
            DateTime now = DateTime.Now;
            string fileFriendlyDate = string.Format("{0}_{1}_{2}", now.Day, now.Month, now.Year);
            return LOG_PATH + "log " + fileFriendlyDate + ".txt";
        }

        /// <summary>
        /// Microsoft documentation for copying directories fomr common I/O tasks.
        /// </summary>
        /// <param name="sourceDirName">Source directory name.</param>
        /// <param name="destDirName">Destination directory name.</param>
        /// <param name="copySubDirs">A bool indicating if subdirectories will be copied. (Default true)</param>
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true, bool cleanCopy = false)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = SourceCheck(sourceDirName);

            // Get directories in the source.
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Check to see the destination directory exists.
            DestinationCheck(destDirName, cleanCopy);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs, cleanCopy);
                }
            }
        }

        /// <summary>
        /// Checks to see if a source directory exists.
        /// </summary>
        /// <param name="sourceDirName">The path of the source.</param>
        /// <returns>A directory info object for the source path.</returns>
        public static DirectoryInfo SourceCheck(string sourceDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("SourceCheck failed for " + sourceDirName);
            }

            return dir;
        }

        public static void UpdateDirectory(string destDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(destDirName);

            if (Directory.Exists(destDirName))
            {
                Directory.Delete(destDirName, true);
                Directory.CreateDirectory(destDirName);
                if (dir.Parent.Name == GAME_BACKUP_DIRECTORY)
                {
                    LogUtil.WriteToLog("Directory recreated at " + destDirName);
                }
            }
        }
    }
}
