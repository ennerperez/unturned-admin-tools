using CommandHandler;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unturned
{
    public static class Freezes
    {
        #region TOP: global variables are initialized here

        internal static Dictionary<String, Vector3> frozenPlayers = new Dictionary<String, Vector3>();

        #endregion

        internal static void GetCommands()
        {
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Freeze, "freeze", "pf")); // Use /freeze <[playername,all]>
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), UnFreeze, "unfreeze", "puf")); // Use /unfreeze <[playername,all]>
        }

        internal static void Freeze(CommandArgs args)
        {
            string naam = args.ParametersAsString;

            if (naam.ToLower() == "all")
            {
                int cdUsers = 0;
                foreach (BetterNetworkUser item in UserList.users)
                {
                    Player player = item.player;
                    frozenPlayers.Add(item.steamid, player.transform.position);
                    //Reference.Tell(item.networkPlayer, msg);
                    cdUsers++;
                }
                Reference.Tell(args.sender.networkPlayer, String.Format("{0} players was frozen.", cdUsers));
            }
            else
            {
                BetterNetworkUser user = UserList.getUserFromName(naam);
                if (user != null)
                {
                    Player player = user.player;
                    frozenPlayers.Add(user.steamid, player.transform.position);
                }
                Reference.Tell(args.sender.networkPlayer, String.Format("Froze the player {0}.", user.name));

            }

        }
        internal static void UnFreeze(CommandArgs args)
        {
            string naam = args.ParametersAsString;

            if (naam.ToLower() == "all")
            {
                int cdUsers = frozenPlayers.Count;
                frozenPlayers.Clear();
                //foreach (BetterNetworkUser item in UserList.users)
                //{
                //    frozenPlayers.Remove(item.steamid);
                //    //Reference.Tell(item.networkPlayer, msg);
                //    cdUsers++;
                //}
                Reference.Tell(args.sender.networkPlayer, String.Format("{0} players was unfrozen.", cdUsers));
            }
            else
            {
                BetterNetworkUser user = UserList.getUserFromName(naam);
                if (user != null)
                {
                    frozenPlayers.Remove(user.steamid);
                }
                Reference.Tell(args.sender.networkPlayer, String.Format("Unfroze the player {0}.", user.name));
            }
        }
        
    }
}
