using CommandHandler;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Unturned
{
    public static class Homes
    {

        #region TOP: global variables are initialized here

        internal static Dictionary<String, Vector3> playerHomes;
        internal static bool usePlayerHomes = false;

        #endregion

        internal static void Load()
        {
            if (usePlayerHomes)
            {
                string fileSource = System.IO.Path.Combine(AdminTools.Path, "homes.txt");
                if (!File.Exists(fileSource))
                {
                    createHomes();
                }

                playerHomes = new Dictionary<String, Vector3>();
                string[] homes = System.IO.File.ReadAllLines(fileSource);
                foreach (string item in homes)
                {
                    try
                    {
                        string[] values = item.Split(':');
                        String id = values[0];
                        String[] location = values[1].Split(',');
                        String x = location[0];
                        String y = location[1];
                        String z = location[2];
                        Vector3 loc = new Vector3(Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(z));
                        playerHomes.Add(id, loc);
                    }
                    catch (Exception ex)
                    {
                        AdminTools.Log(ex);
                    }
                }
            }

        }

        internal static void GetCommands()
        {
            CommandList.add(new Command(PermissionLevel.Level1.ToInt(), SetHome, "sethome", "sh"));
            CommandList.add(new Command(PermissionLevel.Level1.ToInt(), Home, "home", "h"));
        }
                
        internal static void SetHome(CommandArgs args)
        {
            if (usePlayerHomes)
            {
                Vector3 location = NetworkUserList.getModelFromPlayer(args.sender.networkPlayer).transform.position;
                setHome(args.sender.steamid, location);
                Reference.Tell(args.sender.networkPlayer, "Home set.");
            }
        }
        internal static void Home(CommandArgs args)
        {
            if (usePlayerHomes)
            {
                home(args.sender);
                Reference.Tell(args.sender.networkPlayer, "Teleported home. Don't move for 5 seconds if you don't wanna get kicked");
            }
        }

        private static void createHomes()
        {
            System.IO.StreamWriter file = new StreamWriter(System.IO.Path.Combine(AdminTools.Path, "homes.txt"), true);
            file.Close();
        }
        private static void setHome(string steamID, Vector3 location)
        {
            string fileSource = System.IO.Path.Combine(AdminTools.Path, "homes.txt");
            if (playerHomes.ContainsKey(steamID))
            {
                string[] lines = System.IO.File.ReadAllLines(fileSource);
                File.Delete(fileSource);

                System.IO.StreamWriter file = new StreamWriter(fileSource, true);

                foreach (string item in lines)
                {
                    if (!item.StartsWith(steamID))
                    {
                        file.WriteLine(item);
                    }
                }

                file.Close();
            }
            System.IO.StreamWriter file2 = new StreamWriter(fileSource, true);
            file2.WriteLine(String.Format("{0}:{1},{2},{3}", steamID, location.x, location.y, location.z));
            file2.Close();
            playerHomes[steamID] = location;
        }
        private static void home(BetterNetworkUser user)
        {
            Teleports.userTo(user, playerHomes[user.steamid]);
        }

    }
}
