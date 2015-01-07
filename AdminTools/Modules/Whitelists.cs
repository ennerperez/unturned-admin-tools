using CommandHandler;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Unturned
{
    public static class Whitelists
    {
        #region TOP: global variables are initialized here

        private static int updater3 = 0;
        internal static List<String> WhitelistedSteamIDs;
        internal static bool UsingWhitelist = false;
        internal static bool ShowWhiteListKickMessages = true;

        #endregion

        internal static void Load()
        {
            if (UsingWhitelist)
            {
                string fileSource = System.IO.Path.Combine(AdminTools.Path, "whitelist.txt");
                if (!File.Exists(fileSource))
                {
                    create();
                }

                WhitelistedSteamIDs = new List<string>();
                string[] whitelists = System.IO.File.ReadAllLines(fileSource);
                foreach (string item in whitelists)
                {
                    try
                    {
                        WhitelistedSteamIDs.Add(item);
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
            CommandList.add(new Command(PermissionLevel.Admin.ToInt(), Whitelist, "whitelist"));   //used for both /whitelist add & /whitelist remove
            CommandList.add(new Command(PermissionLevel.Admin.ToInt(), Enable, "enablewhitelist"));
            CommandList.add(new Command(PermissionLevel.Admin.ToInt(), Disable, "disablewhitelist"));
        }

        internal static void Enable(CommandArgs args)
        {
            UsingWhitelist = true;
            NetworkChat.sendAlert("Whitelist enabled.");
        }
        internal static void Disable(CommandArgs args)
        {
            UsingWhitelist = false;
            NetworkChat.sendAlert("Whitelist disabled.");
        }
        internal static void Whitelist(CommandArgs args)
        {
            if (args.Parameters[0].Equals("add"))
            {
                args.Parameters.RemoveAt(0);

                String name = args.ParametersAsString;
                string steamid = "";
                try
                {
                    steamid = UserList.getUserFromName(name).steamid;
                    name = UserList.getUserFromName(name).name;
                }
                catch
                {
                    if (name.StartsWith("765611"))
                    {
                        steamid = name;
                    }
                    else
                    {
                        Reference.Tell(args.sender.networkPlayer, "That player is either not online or you provided a bad steamid");
                        return;
                    }

                }

                System.IO.StreamWriter file = new StreamWriter("Unturned_Data/Managed/mods/AdminCommands/UnturnedWhitelist.txt", true);
                file.WriteLine("");
                file.WriteLine(steamid);
                file.Close();
                WhitelistedSteamIDs.Add(steamid);

                Reference.Tell(args.sender.networkPlayer, "Added " + name + " (" + steamid + ") to the whitelist");
            }
            else if (args.Parameters[0].ToLower().Equals("remove") || args.Parameters[0].ToLower().Equals("del") || args.Parameters[0].ToLower().Equals("rem"))
            {
                args.Parameters.RemoveAt(0);

                String name = args.ParametersAsString;
                string steamid = "";
                try
                {
                    steamid = UserList.getUserFromName(name).steamid;
                    name = UserList.getUserFromName(name).name;
                }
                catch
                {
                    if (name.StartsWith("765611"))
                    {
                        steamid = name;
                    }
                    else
                    {
                        Reference.Tell(args.sender.networkPlayer, "That player is either not online or you provided a bad steamid");
                        return;
                    }

                }

                WhitelistedSteamIDs.Remove(steamid);

                File.Delete("Unturned_Data/Managed/mods/AdminCommands/UnturnedWhitelist.txt");
                System.IO.StreamWriter file = new StreamWriter("Unturned_Data/Managed/mods/AdminCommands/UnturnedWhitelist.txt", true);
                for (int i = 0; i < WhitelistedSteamIDs.Count; i++)
                {
                    file.WriteLine(WhitelistedSteamIDs[i]);
                }
                file.Close();

                Reference.Tell(args.sender.networkPlayer, "Removed " + name + " (" + steamid + ") from the whitelist");

            }
        }

        private static void create()
        {
            System.IO.StreamWriter file = new StreamWriter(System.IO.Path.Combine(AdminTools.Path, "whitelist.txt"), true);
            file.Close();
        }

        internal static void KickNonWhitelistedPlayers()
        {
            if (UsingWhitelist && updater3 <= 1)
            {
                foreach (BetterNetworkUser user in UserList.users)
                {
                    if (user.networkPlayer != Network.player && !WhitelistedSteamIDs.Contains(user.steamid))
                    {
                        if (ShowWhiteListKickMessages)
                        {
                            Kicks.kick(user.name, "You are not whitelisted on this server!");
                        }
                        else
                        {
                            NetworkChat.sendNotification(user.networkPlayer, "You are not whitelisted on this server!");
                            Network.CloseConnection(user.networkPlayer, true);
                        }

                    }
                }
                Whitelists.updater3 = 100;
            }
            updater3--;
        }

    }
}
