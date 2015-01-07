using CommandHandler;
using System;
using UnityEngine;

namespace Unturned
{
    public static class Bans
    {
        #region TOP: global variables are initialized here

        private static String bigAssStringWithBannedPlayerNamesAndSteamIDs = "";   //empty until player issues /unban command
        private static BetterNetworkUser userToBeBanned;

        #endregion    

        internal static void Load()
        {
            bigAssStringWithBannedPlayerNamesAndSteamIDs = PlayerPrefs.GetString("bans");
        }

        internal static void GetCommands()
        {
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Ban, "ban"));
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), ReasonForBan, "reason", "rban"));
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), UnBan, "unban", "uban"));
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), ReloadBans, "reloadbans", "lban"));
        }

        internal static void Ban(CommandArgs args)
        {
            String playerNameToBeBanned = args.ParametersAsString;
            String tempBanName;

            userToBeBanned = UserList.getUserFromName(playerNameToBeBanned);

            //int number = 0;
            //string name = "";
            if (userToBeBanned != null) // (playerNameToBeBanned.Length < 3 && Int32.TryParse(name, out number))
            {
                //userToBeBanned = UserList.users[number];
                tempBanName = userToBeBanned.name;
                Reference.Tell(args.sender.networkPlayer, String.Format("Reason for banning {0} ?  /reason <reason> to ban.", tempBanName));

            }
            //else
            //{
            //    //userToBeBanned = UserList.getUserFromName(playerNameToBeBanned);
            //    tempBanName = userToBeBanned.name;
            //    Reference.Tell(args.sender.networkPlayer, String.Format("Reason for banning {0} ?  /reason <reason> to ban.", tempBanName));
            //}
        }
        internal static void UnBan(CommandArgs args)
        {
            String name = args.ParametersAsString;
            if (unban(name))
            {
                Reference.Tell(args.sender.networkPlayer, String.Format("{0} was removed from the ban list.", name));
            }
            else
            {
                Reference.Tell(args.sender.networkPlayer, String.Format("Could not find {0} in the list of banned players.", name));
            }
        }
        internal static void ReasonForBan(CommandArgs args)
        {
            String reason = args.ParametersAsString;
            if (args.Parameters.Count > 0)
                ban(userToBeBanned, reason);
        }
        internal static void ReloadBans(CommandArgs args)
        {
            NetworkBans.load();
        }
        
        private static void saveBans()
        {
            PlayerPrefs.SetString("bans", bigAssStringWithBannedPlayerNamesAndSteamIDs);
        }

        private static void ban(BetterNetworkUser userToBeBanned, string reason)
        {
            NetworkTools.ban(userToBeBanned.networkPlayer, userToBeBanned.name, userToBeBanned.steamid, reason);
        }
        private static bool unban(String name)
        {
            Load();
            string bannedppl = bigAssStringWithBannedPlayerNamesAndSteamIDs;

            if (bannedppl.Contains(name))
            {
                int startIndex = bannedppl.IndexOf(name);

                int length = name.Length + 1 + 17 + 2;

                String temp1 = bannedppl.Substring(0, startIndex);
                String temp2 = bannedppl.Substring(startIndex + length);
                bannedppl = temp1 + temp2;
            }

            else
            {
                return false;
            }

            bigAssStringWithBannedPlayerNamesAndSteamIDs = bannedppl;
            saveBans();
            NetworkBans.load();
            return true;
        }

    }
}
