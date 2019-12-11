using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BackupSaves
{
    public class Program
    {
        /// <summary>
        /// Constant that contains the backup directory path.
        /// </summary>
        private const string BASE_BACKUP_DIRECTORY = @"D:\SavesBackup\";

        private const string LOG_PATH = BASE_BACKUP_DIRECTORY + "log.txt";

        /// <summary>
        /// A write lock object to handle system I/O exceptions with the log file.
        /// </summary>
        private static ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

        /// <summary>
        /// A dictionary object that contains game names(key) and their corresponding saves path(value).
        /// </summary>
        private static readonly Dictionary<string, string> SaveLocations = new Dictionary<string, string>
        {
            { "Grim Dawn", @"C:\Program Files (x86)\Steam\userdata\62462215\219990\remote\save" },
            { "Monster Hunter World", @"C:\Program Files (x86)\Steam\userdata\62462215\582010\remote" },
            { "AC Odyssey", @"C:\Program Files (x86)\Ubisoft\Ubisoft Game Launcher\savegames\7e7ad2e9-f90b-494e-a1cc-c4ec784c9b3c\5092" },
            { "AC3", @"C:\Program Files (x86)\Ubisoft\Ubisoft Game Launcher\savegames\7e7ad2e9-f90b-494e-a1cc-c4ec784c9b3c\5184" }
        };

        /// <summary>
        /// A list object that contains the menu items for the console window.
        /// </summary>
        private static readonly List<string> Commands = new List<string>
        {
            "1 - List Game Saves[NYI]",
            "2 - Backup All Game Saves",
            "3 - Add Game Save Location[NYI]",
            "4 - Remove Game Save Location[NYI]",
            "5 - Backup Specific Game Saves[NYI]",
            "6 - Help[NYI]",
            "9 - Reset",
            "0 - Exit"
        };


        /// <summary>
        /// Executes the console program.
        /// </summary>
        /// <param name="args">Passed in args from the console.</param>
        public static void Main(string[] args)
        {
            // Program Initialization
            DestinationCheck(BASE_BACKUP_DIRECTORY, false);

            if (!File.Exists(LOG_PATH))
            {
                File.Create(LOG_PATH);
            }

            Menu();

            // Input functionality
            ConsoleKey input = Console.ReadKey().Key;
            while (input != ConsoleKey.D0)
            {
                Console.Write("\n");
                switch (input)
                {
                    case ConsoleKey.D2:
                        Backup();
                        break;
                    case ConsoleKey.D9:
                        Console.Clear();
                        Menu();
                        break;
                    default:
                        Console.WriteLine("Command not recognized, enter 6 for help, 9 to reset, or 0 to exit.");
                        break;
                }

                input = Console.ReadKey().Key;
            }
        }

        /// <summary>
        /// Check to see that the backup directory exists; Creates the directory if it doesn't
        /// </summary>
        /// <param name="destDirName">Destination directory path.</param>
        /// <param name="subDir">Indicates if the directory is a subdirectory of the master (Default true)</param>
        private static void DestinationCheck(string destDirName, bool subDir = true)
        {
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            else if (subDir)
            {
                Directory.Delete(destDirName, true);
                Directory.CreateDirectory(destDirName);
            }
        }

        /// <summary>
        /// Runs the backup process looping through the dictionary of games and save locations.
        /// </summary>
        private static void Backup()
        {
            try
            {
                foreach (KeyValuePair<string, string> location in SaveLocations)
                {
                    string game = location.Key.ToString();
                    string source = location.Value.ToString();

                    DirectoryCopy(source, BASE_BACKUP_DIRECTORY + game);

                    // Output results
                    string result = string.Format("{0} backup successful.", game);
                    Console.WriteLine(result);
                    WriteToLog(result);
                }
                Console.WriteLine("Backup Complete");
                WriteToLog("Backup Completed");
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("\n{0}\n{1}", e.Message, e.StackTrace));
                WriteToLog(e.Message);
            }
        }
        
        /// <summary>
        /// Writes a string to a log.txt file in the backup directory.
        /// </summary>
        /// <param name="log">Log message being written to the log.txt file.</param>
        private static void WriteToLog(string log)
        {
            // Set status to locked.
            _readWriteLock.EnterWriteLock();

            try
            {
                // Append log to file.
                using (var file = new StreamWriter(LOG_PATH, true))
                {
                    file.WriteLine(string.Format("[{0}]: {1}", DateTime.Now, log));
                    file.Close();
                }
            }
            finally
            {
                // Release lock.
                _readWriteLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// A method that displays a consol menu of actions for the backup file system.
        /// </summary>
        /// <returns>The console key that is inputed after the menu is redisplayed.</returns>
        private static void Menu()
        {
            string header = string.Format("{0}\n{1}\n", 
                                          "Saved Games Backup Console", 
                                          "--------------------------");
            Console.WriteLine(header);
            Commands.ForEach(command => Console.WriteLine(command));
        }

        /// <summary>
        /// Microsoft documentation for copying directories fomr common I/O tasks.
        /// </summary>
        /// <param name="sourceDirName">Source directory name.</param>
        /// <param name="destDirName">Destination directory name.</param>
        /// <param name="copySubDirs">A bool indicating if subdirectories will be copied. (Default true)</param>
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            try
            {
                // Get the subdirectories for the specified directory.
                DirectoryInfo dir = SourceCheck(sourceDirName);

                // Get directories in the source.
                DirectoryInfo[] dirs = dir.GetDirectories();

                // Check to see the destination directory exists.
                DestinationCheck(destDirName);

                // Get the files in the directory and copy them to the new location.
                CopyFiles(destDirName, dir);

                // If copying subdirectories, copy them and their contents to new location.
                if (copySubDirs)
                {
                    foreach (DirectoryInfo subdir in dirs)
                    {
                        string temppath = Path.Combine(destDirName, subdir.Name);
                        DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("\n{0}\n{1}", e.Message, e.StackTrace));
                WriteToLog(e.Message);
            }

        }

        private static DirectoryInfo SourceCheck(string sourceDirName)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            return dir;
        }

        private static void CopyFiles(string destDirName, DirectoryInfo dir)
        {
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }
        }
    }
}
