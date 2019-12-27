using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace BackupSaves
{
    public class Program
    {
        private static int fileError = 0;

        private static int backupError = 0;

        /// <summary>
        /// A list object that contains the menu items for the console window.
        /// </summary>
        private static readonly List<string> Commands = new List<string>
        {
            "1 - List Games[NYI]",
            "2 - Backup All Game Saves",
            "5 - Backup Specific Game Saves[NYI]",
            "6 - Help",
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
            FileUtil.DestinationCheck(FileUtil.BASE_BACKUP_DIRECTORY, false);

            using(FileStream stream = new FileStream(FileUtil.LOG_PATH, FileMode.Create))
            {
                if (!File.Exists(FileUtil.LOG_PATH))
                {
                    stream.WriteByte(new byte());
                    stream.Flush();
                }
                stream.Close();
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
                        BackupSummary(FileUtil.Backup());
                        break;
                    case ConsoleKey.D6:
                        Help();
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
        /// 
        /// </summary>
        private static void Help()
        {
            Console.WriteLine("This console utilizes a simple single key interface.");
            Console.WriteLine("To perform an action, press the corresponding key from the menu");
            // Add readkey here to provide help for individual commands.
        }

        /// <summary>
        /// Provides a console output warning of errors during the backup process.
        /// </summary>
        /// <param name="errorCount">The number of errors.</param>
        private static void BackupSummary(int errorCount)
        {
            if(errorCount > 0)
            {
                Console.WriteLine($"There were {errorCount} errors while backing up. Check the log file created in the backup location for more details.");
            }
            else
            {
                Console.WriteLine("Backup Complete with no errors.");
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
    }
}
