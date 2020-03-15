using System;
using System.Collections.Generic;
using System.IO;
using static BackupSaves.Utilities.APIUtil;
using static BackupSaves.Utilities.FileUtil;
using static BackupSaves.Utilities.LogUtil;

namespace BackupSaves.Utilities
{
    public class BackupUtil
    {
        private static void Complete() => Console.Write("Complete");
        private static void Error() => Console.Write("Error, see log.txt for details.");

        /// <summary>
        /// A dictionary object that contains uplay game names(key) and their corresponding saves path(value).
        /// </summary>
        private static readonly Dictionary<int, string> UplaySaves = new Dictionary<int, string>
        {
            { 5092, "Assassin's Creed Odyssey" },
            { 5184, "Assassin's Creed 3" }
        };

        /// <summary>
        /// A dictionary object that contains steam game names(key) and their corresponding saves path(value).
        /// </summary>
        private static readonly Response SteamSaves = GetSteamApps().Result;

        /// <summary>
        /// Runs the backup process looping through the dictionary of games and save locations.
        /// </summary>
        public static void Backup()
        {
            List<Game> filteredSaves = new List<Game>();

            try
            {
                Console.Write("Starting source path check...");
                // Check the existance of source save location paths.
                DirectoryInfo steam = SourceCheck(STEAM_LOCATION);
                DirectoryInfo uplay = SourceCheck(UPLAY_LOCATION);
                Complete();

                Console.Write("\nGathering Steam game information...");
                // Grab contents of sources to get the game IDs to filter the api call.
                DirectoryInfo[] steamDirs = steam.GetDirectories();
                foreach (DirectoryInfo item in steamDirs)
                {
                    Game game = SteamSaves.games.Find(x => x.appid.ToString() == item.Name);
                    if (game != null)
                    {
                        filteredSaves.Add(game);
                    }
                }
                Complete();
            }
            catch (DirectoryNotFoundException e)
            {
                Error();
                WriteToLog(string.Format("{0}\n{1}", e.Message, e.StackTrace.Substring(0, e.StackTrace.IndexOf(" in C:"))));
            } 

            // Backup process
            try
            {
                Console.Write("\nPreparing destination directory...");
                DestinationCheck(GAME_BACKUP_DIRECTORY, true);
                Complete();

                Console.Write("\nStarting Steam backup...");
                foreach (Game game in filteredSaves)
                {
                    string id = game.appid.ToString();
                    string name = string.Join("_", game.name.Split(Path.GetInvalidFileNameChars()));

                    DirectoryCopy(STEAM_LOCATION + id, GAME_BACKUP_DIRECTORY + name);
                }
                Complete();

                Console.Write("\nStarting Uplay backup...");
                foreach (KeyValuePair<int, string> save in UplaySaves)
                {
                    string id = save.Key.ToString();
                    string name = string.Join("_", save.Value.ToString().Split(Path.GetInvalidFileNameChars()));

                    DirectoryCopy(UPLAY_LOCATION + id, GAME_BACKUP_DIRECTORY + name);
                }
                Complete();
            }
            catch (Exception e)
            {
                Error();
                WriteToLog(string.Format("{0}\n{1}", e.Message, e.StackTrace.Substring(0, e.StackTrace.IndexOf(" in C:"))));
            }

            WriteToLog("Backup Process Finished.");
        }
    }
}
