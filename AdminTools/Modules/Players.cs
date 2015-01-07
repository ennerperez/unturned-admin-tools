using CommandHandler;
using System;
using UnityEngine;

namespace Unturned
{
    public static class Players
    {

        #region TOP: global variables are initialized here

        #endregion

        internal static void GetCommands()
        {
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Spawn, "player", "p")); // Use /player <[playername]>
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Respawn, "respawnplayers", "pr"));
            CommandList.add(new Command(PermissionLevel.SuperUser.ToInt(), Skill, "skill", "ps")); // Use skill <[playername,all]>
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Kill, "kill", "pk")); // Use /kill <[playername,all,others]>
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Heal, "heal", "ph")); // Use /heal <[playername,all,others]>
        }

        internal static void Spawn(CommandArgs args)
        {
            String naam = args.ParametersAsString;
            if (String.IsNullOrEmpty(naam.Trim())) { naam = args.sender.player.name; }

            BetterNetworkUser user = UserList.getUserFromName(naam);
            if (user != null)
            {
                Transform transform = SpawnPlayers.getSpawnPoint(user.networkPlayer, true);
                Teleports.userTo(user, transform.position);
                Reference.Tell(user.networkPlayer, String.Format("You was spawned."));
            }
            else
            {
                Reference.Tell(args.sender.networkPlayer, "Player was not found.");
            }

        }

        internal static void Respawn(CommandArgs args)
        {
            BetterNetworkUser[] mapUsers = UserList.users.ToArray();
            foreach (BetterNetworkUser item in mapUsers)
            {
                Transform transform = SpawnPlayers.getSpawnPoint(item.networkPlayer, true);
                Teleports.userTo(item, transform.position);
            }
            NetworkChat.sendAlert(String.Format("{0} has re-spawned {0} payers.", args.sender.name, mapUsers.Length));
        }

        internal static void Kill(CommandArgs args)
        {
            String naam = args.ParametersAsString;
            String msg = "You died of retardedness"; // args.Parameters[args.Parameters.Count - 1];

            if ((naam.Trim().ToLower() == "all") || (naam.Trim().ToLower() == "others"))
            {
                int cdUsers = 0;

                if (naam.Trim().ToLower() == "all")
                {
                    foreach (BetterNetworkUser item in UserList.users)
                    {
                        item.player.gameObject.GetComponent<Life>().damage(10000, msg);
                        cdUsers++;
                    }
                }

                if (naam.Trim().ToLower() == "others")
                {
                    foreach (BetterNetworkUser item in UserList.users)
                    {
                        if (item.name != args.sender.name)
                        {
                            item.player.gameObject.GetComponent<Life>().damage(10000, msg);
                            cdUsers++;
                        }
                    }
                }

                if (cdUsers > 4)
                {
                    NetworkChat.sendAlert(String.Format("This was a genocide, {0} users was killed.", cdUsers));
                }
                else
                {
                    if (cdUsers > 0) { NetworkChat.sendAlert(String.Format("This was a sacrifice, {0} users was killed.", cdUsers)); }
                }

            }
            else
            {
                if (String.IsNullOrEmpty(naam.Trim())) { naam = args.sender.player.name; }
                BetterNetworkUser user = UserList.getUserFromName(naam);
                if (user != null)
                {
                    user.player.gameObject.GetComponent<Life>().damage(10000, msg);
                    Reference.Tell(user.networkPlayer, "Head shot!");
                }
                else
                {
                    Reference.Tell(args.sender.networkPlayer, "Player was not found.");
                }
            }

        }
        internal static void Heal(CommandArgs args)
        {
            String naam = args.ParametersAsString;
            String msg = "Lord Jesus Christ heal you soul and body.";

            if ((naam.Trim().ToLower() == "all") || (naam.Trim().ToLower() == "others"))
            {
                int cdUsers = 0;
                if (naam.Trim().ToLower() == "all")
                {
                    foreach (BetterNetworkUser item in UserList.users)
                    {
                        item.player.gameObject.GetComponent<Life>().heal(100, true, true);
                        Reference.Tell(item.networkPlayer, msg);
                        cdUsers++;
                    }
                }

                if (naam.Trim().ToLower() == "others")
                {
                    foreach (BetterNetworkUser item in UserList.users)
                    {
                        if (item.name != args.sender.name)
                        {
                            item.player.gameObject.GetComponent<Life>().heal(100, true, true);
                            Reference.Tell(item.networkPlayer, msg);
                            cdUsers++;
                        }
                    }
                }

                if (cdUsers > 0) { NetworkChat.sendAlert(String.Format("This was a miracle!, {0} users was healed.", cdUsers)); }

            }
            else
            {
                if (String.IsNullOrEmpty(naam.Trim())) { naam = args.sender.player.name; }
                BetterNetworkUser user = UserList.getUserFromName(naam);
                if (user != null)
                {
                    user.player.gameObject.GetComponent<Life>().heal(100, true, true);
                    Reference.Tell(user.networkPlayer, msg);
                }
                else
                {
                    Reference.Tell(args.sender.networkPlayer, "Player was not found.");
                }
            }

        }
        internal static void Skill(CommandArgs args)
        {
            String naam = args.ParametersAsString;
            int amount = 10000;

            string[] ns = args.ParametersAsString.Split(' ');
            if (ns.Length >1) { amount =  int.Parse( ns[ns.Length]); }
            if (amount <= 0) { return; }

            String msg = "You has been blessed, you learned {0} skill points.";

            if ((naam.Trim().ToLower() == "all") || (naam.Trim().ToLower() == "others"))
            {
                int cdUsers = 0;
                if (naam.Trim().ToLower() == "all")
                {
                    foreach (BetterNetworkUser item in UserList.users)
                    {
                        item.player.GetComponent<Skills>().learn(amount);
                        Reference.Tell(item.networkPlayer, String.Format(msg, amount));
                        cdUsers++;
                    }
                }

                if (naam.Trim().ToLower() == "others")
                {
                    foreach (BetterNetworkUser item in UserList.users)
                    {
                        if (item.name != args.sender.name)
                        {
                            item.player.GetComponent<Skills>().learn(amount);
                            Reference.Tell(item.networkPlayer, String.Format(msg, amount));
                            cdUsers++;
                        }
                    }
                }

                if (cdUsers > 0) { NetworkChat.sendAlert(String.Format("All has been blessed, {0} users was learned {1} skill points.", cdUsers, amount)); }

            }
            else
            {
                if (String.IsNullOrEmpty(naam.Trim())) { naam = args.sender.player.name; }
                BetterNetworkUser user = UserList.getUserFromName(naam);
                if (user != null)
                {
                    user.player.GetComponent<Skills>().learn(amount);
                    Reference.Tell(user.networkPlayer, String.Format(msg, amount));
                }
                else
                {
                    Reference.Tell(args.sender.networkPlayer, "Player was not found.");
                }
            }


        }

        internal static void Promote(CommandArgs args)
        {
            String naam = args.ParametersAsString;
            BetterNetworkUser user = UserList.getUserFromName(naam);

            UserList.promote(user.steamid);

            Reference.Tell(args.sender.networkPlayer, String.Format("{0} has been promoted to level {1}", naam, UserList.getPermission(user.steamid)));
            Reference.Tell(user.networkPlayer, string.Format("You have been promoted to level {0}", UserList.getPermission(user.steamid)));
        }

    }
}
