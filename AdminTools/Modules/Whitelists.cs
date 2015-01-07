using CommandHandler;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Unturned
{
    internal class Whitelists : Module
    {

        #region TOP: global variables are initialized here

        private static int updater3 = 0;
        internal static List<String> WhitelistedSteamIDs;
        
        internal static bool UseWhitelist = false;
        internal static bool ShowWhiteListKickMessages = true;

        private static string fileSource = System.IO.Path.Combine(AdminTools.Path, "whitelist.txt");

        #endregion

        internal override void Load()
        {
            if (String.IsNullOrEmpty(Configs.File.IniReadValue("Config", "UseWhitelist")))
            {
                Configs.File.IniWriteValue("Config", "UseWhitelist", "false");
                Configs.File.IniWriteValue("Config", "ShowWhiteListKickMessages", "true");
            }

            Whitelists.UseWhitelist = Boolean.Parse(Configs.File.IniReadValue("Config", "UseWhitelist"));
            Whitelists.ShowWhiteListKickMessages = Boolean.Parse(Configs.File.IniReadValue("Config", "ShowWhiteListKickMessages"));

            if (Whitelists.UseWhitelist)
            {

                if (!File.Exists(fileSource)) { Create();}

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
                        Shared.Log(ex.ToString());
                    }
                }
            }
        }
        internal override void Create()
        {
            System.IO.StreamWriter file = new StreamWriter(fileSource, true);
            file.Close();
        }

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), Whitelist, "whitelist"));   //used for both /whitelist add & /whitelist remove
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), Enable, "enablewhitelist"));
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), Disable, "disablewhitelist"));
            return _return;
        }
        internal override String GetHelp()
        {
            return null;
        }

        #region Commands

        internal static void Enable(CommandArgs args)
        {
            UseWhitelist = true;
            NetworkChat.sendAlert("Whitelist enabled.");
        }
        internal static void Disable(CommandArgs args)
        {
            UseWhitelist = false;
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

        #endregion

        #region Private calls

        internal static void KickNonWhitelistedPlayers()
        {
            if (UseWhitelist && updater3 <= 1)
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

        #endregion

    }
}
