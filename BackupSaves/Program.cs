using System;
using BackupSaves.Utilities;

namespace BackupSaves
{
    public class Program
    {
        /// <summary>
        /// Executes the console program.
        /// </summary>
        /// <param name="args">Passed in args from the console.</param>
        public static void Main(string[] args)
        {
            // Program Initialization
            FileUtil.DestinationCheck(FileUtil.BASE_BACKUP_DIRECTORY, false);
            FileUtil.InitiateLogFile();
            ConsoleUtil.Menu();

            // Input functionality
            while (ConsoleUtil.GetInput() != ConsoleKey.Escape) { }

        }
    }
}
