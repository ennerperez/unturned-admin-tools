using CommandHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Unturned
{
    internal class Whitelists : Module
    {

        #region TOP: global variables are initialized here

        private static int updater3 = 0;
        internal static List<String> WhitelistedSteamIDs;

        internal static bool UseWhitelists = false;
        internal static bool KickMessages = true;

        private static string Source = System.IO.Path.Combine(AdminTools.AdminPath, "whitelists.txt");

        #endregion

        internal override void Load()
        {
            if (String.IsNullOrEmpty(Configs.File.IniReadValue("Modules", "Whitelists")))
            {
                Configs.File.IniWriteValue("Modules", "Whitelists", "false");
                Configs.File.IniWriteValue("Config", "KickMessages", "true");
            }

            Whitelists.UseWhitelists = Boolean.Parse(Configs.File.IniReadValue("Modules", "Whitelists"));
            Whitelists.KickMessages = Boolean.Parse(Configs.File.IniReadValue("Config", "KickMessages"));

            if (Whitelists.UseWhitelists)
            {

                if (!File.Exists(Source)) { Create(); }

                WhitelistedSteamIDs = new List<string>();
                string[] whitelists = System.IO.File.ReadAllLines(Source);
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
        internal override void Refresh()
        {
            if (Whitelists.UseWhitelists)
            {
                Whitelists.kicks();
            }

        }
        internal override void Create()
        {
            System.IO.StreamWriter file = new StreamWriter(Source, true);
            file.Close();
        }
        internal override void Clear()
        {
            Whitelists.WhitelistedSteamIDs = null;
        }

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), Whitelist, "whitelist"));
            return _return;
        }
        internal override String GetHelp()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Strings.Get("HLP", "WhitelistHelp"));

            return sb.ToString();
        }

        #region Commands

        internal static void Whitelist(CommandArgs args)
        {

            if (args.Parameters.Count == 0) { return; }

            string command = args.Parameters[0].Trim().ToLower();

            switch (command)
            {
                case "on":
                    UseWhitelists = true;
                    NetworkChat.sendAlert(Strings.Get("MOD", "WhitelistOn"));
                    break;

                case "off":
                    UseWhitelists = false;
                    NetworkChat.sendAlert(Strings.Get("MOD", "WhitelistOff"));
                    break;

                case "add":
                    if (UseWhitelists)
                    {
                        args.Parameters.RemoveAt(0);
                        BetterNetworkUser adduser = UserList.getUserFromName(args.ParametersAsString);

                        if (add(adduser))
                        {
                            Reference.Tell(args.sender.networkPlayer, String.Format(Strings.Get("MOD", "WhitelistAdded"), adduser.name, adduser.steamid));
                        }
                        else
                        {
                            Reference.Tell(args.sender.networkPlayer, Strings.Get("MOD", "WhitelistPlayerNotFound"));
                        }
                    }
                    else
                    {
                        Reference.Tell(args.sender.networkPlayer, Strings.Get("MOD", "WhitelistOff"));
                    }
                    break;

                case "del":
                    if (UseWhitelists)
                    {
                        args.Parameters.RemoveAt(0);
                        BetterNetworkUser deluser = UserList.getUserFromName(args.ParametersAsString);

                        if (del(deluser))
                        {
                            Reference.Tell(args.sender.networkPlayer, String.Format(Strings.Get("MOD", "WhitelistRemoved"), deluser.name, deluser.steamid));
                        }
                        else
                        {
                            Reference.Tell(args.sender.networkPlayer, Strings.Get("MOD", "WhitelistPlayerNotFound"));
                        }
                    }
                    else
                    {
                        Reference.Tell(args.sender.networkPlayer, Strings.Get("MOD", "WhitelistOff"));
                    }
                    break;

                default:
                    break;
            }

        }

        #endregion

        private static bool add(BetterNetworkUser user)
        {

            string name = "";
            string steamid = "";

            try
            {
                steamid = user.steamid;
                name = user.name;
            }
            catch
            {
                if (name.StartsWith("765611"))
                {
                    steamid = name;
                }
                else
                {
                    return false;
                }

            }

            System.IO.StreamWriter file = new StreamWriter(Source, true);
            file.WriteLine("");
            file.WriteLine(steamid);
            file.Close();
            WhitelistedSteamIDs.Add(steamid);


            return true;

        }
        private static bool del(BetterNetworkUser user)
        {

            string name = "";
            string steamid = "";

            try
            {
                steamid = user.steamid;
                name = user.name;
            }
            catch
            {
                if (name.StartsWith("765611"))
                {
                    steamid = name;
                }
                else
                {
                    return false;
                }

            }

            WhitelistedSteamIDs.Remove(steamid);

            File.Delete(Source);
            System.IO.StreamWriter file = new StreamWriter(Source, true);
            for (int i = 0; i < WhitelistedSteamIDs.Count; i++)
            {
                file.WriteLine(WhitelistedSteamIDs[i]);
            }
            file.Close();


            return true;

        }

        private static void kicks()
        {
            if (UseWhitelists && updater3 <= 1)
            {
                foreach (BetterNetworkUser user in UserList.users)
                {
                    if (user.networkPlayer != Network.player && !WhitelistedSteamIDs.Contains(user.steamid))
                    {
                        if (KickMessages)
                        {
                            Kicks.kick(user.name, Strings.Get("MOD","WhitelistNotWhitelisted"));
                        }
                        else
                        {
                            NetworkChat.sendNotification(user.networkPlayer, Strings.Get("MOD", "WhitelistNotWhitelisted"));
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
