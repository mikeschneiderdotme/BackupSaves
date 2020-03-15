using System;
using System.Collections.Generic;

namespace BackupSaves.Utilities
{
    public static class ConsoleUtil
    {
        /// <summary>
        /// A list object that contains the menu items for the console window.
        /// </summary>
        private static readonly List<string> Commands = new List<string>
        {
            "2 - Backup All Game Saves",
            "6 - Help",
            "9 - Reset Window",
            "Esc - Exit"
        };

        /// <summary>
        /// A method that displays a consol menu of actions for the backup file system.
        /// </summary>
        /// <returns>The console key that is inputed after the menu is redisplayed.</returns>
        public static void Menu()
        {
            string header = string.Format("{0}\n{1}\n",
                                          "-------Command Menu-------",
                                          "--------------------------");
            Console.WriteLine(header);
            Commands.ForEach(command => Console.WriteLine(command));
        }

        public static ConsoleKey GetInput()
        {
            Console.Write("\n>");

            ConsoleKey input = Console.ReadKey().Key;

            Console.Write("\n");

            switch (input)
            {
                case ConsoleKey.D2:
                    BackupUtil.Backup();
                    break;
                case ConsoleKey.D6:
                    Help();
                    break;
                case ConsoleKey.D9:
                    Console.Clear();
                    Menu();
                    break;
                default:
                    Console.WriteLine("Command not recognized, enter 6 for help, 9 to reset, or Esc to exit.");
                    break;
            }

            return input;
        }

        public static void InitializeConsole()
        {
            Console.WriteLine("----------------------------------");
            Console.WriteLine("-Backup Saves Console Application-");
            Console.WriteLine("---Created by Michael Schneider---");
            Console.WriteLine("----------------------------------\n");
            Menu();
        }

        /// <summary>
        /// Provides decriptive help for the menu functions of this program.
        /// </summary>
        private static void Help()
        {
            Console.WriteLine("This console utilizes a simple single key interface.");
            Console.WriteLine("To perform an action, press the corresponding key from the menu");
            // [NYI] Add readkey here to provide help for individual commands.
        }
    }
}
