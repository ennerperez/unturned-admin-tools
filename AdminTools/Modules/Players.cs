using CommandHandler;
using Ini;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Unturned
{
    internal class Players : Module
    {

        #region TOP: global variables are initialized here

        private static string Source = System.IO.Path.Combine(AdminTools.ModsPath, "UserPermissionLevels.ini");
        internal static IniFile File = new IniFile(Players.Source);

        #endregion

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Respawn, "respawn", "pr"));
            _return.Add(new Command(PermissionLevel.SuperUser.ToInt(), Skill, "skill", "ps"));
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Kill, "kill", "pk"));
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Heal, "heal", "ph"));

            _return.Add(new Command(PermissionLevel.Admin.ToInt(), Promote, "promote"));
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), Demote, "demote"));

            return _return;
        }
        internal override String GetHelp()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Strings.Get("HLP", "PlayersRespawnHelp"));
            sb.AppendLine(Strings.Get("HLP", "PlayersSkillHelp"));
            sb.AppendLine(Strings.Get("HLP", "PlayersKillHelp"));
            sb.AppendLine(Strings.Get("HLP", "PlayersHealHelp"));
            sb.AppendLine(Strings.Get("HLP", "PlayersPromoteHelp"));
            sb.AppendLine(Strings.Get("HLP", "PlayersDemoteHelp"));

            return sb.ToString();
        }

        #region Commands

        internal static void Respawn(CommandArgs args)
        {

            String naam = args.ParametersAsString;
            String msg = Strings.Get("MOD", "PlayerRespawnDefault");

            int respawns = 0;

            if (naam.Trim().ToLower() == "all")
            {
                respawns = respawn(null, true, msg);
            }
            else if (naam.Trim().ToLower() == "others")
            {
                respawns = respawn(null, true, msg, new BetterNetworkUser[] { args.sender });
            }
            else
            {
                if (String.IsNullOrEmpty(naam.Trim())) { naam = args.sender.player.name; }
                BetterNetworkUser user = UserList.getUserFromName(naam);
                respawns = respawn(user, true, msg, null);
            }

            if (respawns == 0)
            {
                Reference.Tell(args.sender.networkPlayer, Strings.Get("MOD", "PlayerNotFound"));
            }
            else
            {
                NetworkChat.sendAlert(String.Format(Strings.Get("MOD", "PlayersRespawn"), args.sender.name, respawns));
            }

        }
        internal static void Skill(CommandArgs args)
        {
            String naam = args.ParametersAsString;
            String msg = Strings.Get("MOD", "PlayerSkillDefault");
            int amount = 10000;

            string[] ns = args.ParametersAsString.Split(' ');
            if (ns.Length > 1) { amount = int.Parse(ns[ns.Length]); }
            if (amount <= 0) { return; }

            int skills = 0;

            if (naam.Trim().ToLower() == "all")
            {
                skills = skill(null, amount, true, msg);
            }
            else if (naam.Trim().ToLower() == "others")
            {
                skills = skill(null, amount, true, msg, new BetterNetworkUser[] { args.sender });
            }
            else
            {
                if (String.IsNullOrEmpty(naam.Trim())) { naam = args.sender.player.name; }
                BetterNetworkUser user = UserList.getUserFromName(naam);
                skills = skill(user, amount, true, msg, null);
            }


            if (skills == 0)
            {
                Reference.Tell(args.sender.networkPlayer, Strings.Get("MOD", "PlayerNotFound"));
            }
            else if (skills == 1)
            {
                Reference.Tell(args.sender.networkPlayer, String.Format(Strings.Get("MOD", "PlayerSkilled"), naam));
            }
            else if (skills > 1)
            {
                NetworkChat.sendAlert(String.Format(Strings.Get("MOD", "PlayersBlessed"), skills, amount));
            }

        }
        internal static void Kill(CommandArgs args)
        {
            String naam = args.ParametersAsString;
            String msg = Strings.Get("MOD", "PlayerKillDefault");


            int kills = 0;

            if (naam.Trim().ToLower() == "all")
            {
                kills = kill(null, true, msg);
            }
            else if (naam.Trim().ToLower() == "others")
            {
                kills = kill(null, true, msg, new BetterNetworkUser[] { args.sender });
            }
            else
            {
                if (String.IsNullOrEmpty(naam.Trim())) { naam = args.sender.player.name; }
                BetterNetworkUser user = UserList.getUserFromName(naam);
                kills = kill(user, true, msg, null);
            }


            if (kills == 0)
            {
                Reference.Tell(args.sender.networkPlayer, Strings.Get("MOD", "PlayerNotFound"));
            }
            else if (kills == 1)
            {
                Reference.Tell(args.sender.networkPlayer, String.Format(Strings.Get("MOD", "PlayerKilled"), naam));
            }
            else if (kills > 4)
            {
                NetworkChat.sendAlert(String.Format(Strings.Get("MOD", "PlayersGenocide"), kills));
            }
            else if (kills > 0)
            {
                NetworkChat.sendAlert(String.Format(Strings.Get("MOD", "PlayersSacrifice"), kills));
            }

        }
        internal static void Heal(CommandArgs args)
        {
            String naam = args.ParametersAsString;
            String msg = Strings.Get("MOD", "PlayerHealDefault");


            int heals = 0;

            if (naam.Trim().ToLower() == "all")
            {
                heals = heal(null, true, msg);
            }
            else if (naam.Trim().ToLower() == "others")
            {
                heals = heal(null, true, msg, new BetterNetworkUser[] { args.sender });
            }
            else
            {
                if (String.IsNullOrEmpty(naam.Trim())) { naam = args.sender.player.name; }
                BetterNetworkUser user = UserList.getUserFromName(naam);
                heals = heal(user, true, msg, null);
            }


            if (heals == 0)
            {
                Reference.Tell(args.sender.networkPlayer, Strings.Get("MOD", "PlayerNotFound"));
            }
            else if (heals == 1)
            {
                Reference.Tell(args.sender.networkPlayer, String.Format(Strings.Get("MOD", "PlayerHealed"), naam));
            }
            else if (heals > 1)
            {
                NetworkChat.sendAlert(String.Format(Strings.Get("MOD", "PlayersMiracle"), heals));
            }

        }

        internal static void Promote(CommandArgs args)
        {
            String naam = args.ParametersAsString;
            BetterNetworkUser user = UserList.getUserFromName(naam);

            if (user != null) {
                int lvl = promote(user);
                if (lvl != -1)
                {
                    Reference.Tell(args.sender.networkPlayer, String.Format(Strings.Get("MOD", "PlayersPromote"), naam, UserList.getPermission(user.steamid)));
                    Reference.Tell(user.networkPlayer, string.Format(Strings.Get("MOD", "PlayerPromoted"), UserList.getPermission(user.steamid)));
                }
            }
            else
            {
                Reference.Tell(args.sender.networkPlayer, Strings.Get("MOD", "PlayerNotFound"));
            }

        }
        internal static void Demote(CommandArgs args)
        {
            String naam = args.ParametersAsString;
            BetterNetworkUser user = UserList.getUserFromName(naam);
            
            if (user != null)
            {
                int lvl = demote(user);
                if (lvl != -1)
                {
                    Reference.Tell(args.sender.networkPlayer, String.Format(Strings.Get("MOD", "PlayersDemote"), naam, UserList.getPermission(user.steamid)));
                    Reference.Tell(user.networkPlayer, string.Format(Strings.Get("MOD", "PlayerDemoted"), UserList.getPermission(user.steamid)));
                }
            }
            else
            {
                Reference.Tell(args.sender.networkPlayer, Strings.Get("MOD", "PlayerNotFound"));
            }


        }

        #endregion

        private static int respawn(BetterNetworkUser user, bool all = false, string msg = "", BetterNetworkUser[] exclude = null)
        {
            try
            {
                BetterNetworkUser[] mapUsers = UserList.users.ToArray();
                List<BetterNetworkUser> exUsers = null;

                if ((exclude != null) && exclude.Length > 0)
                {
                    exUsers = new List<BetterNetworkUser>(exclude);
                }

                if (all)
                {
                    int counter = 0;
                    foreach (BetterNetworkUser User in mapUsers)
                    {
                        if ((exUsers != null) && exUsers.Count > 0)
                        {
                            if (!exUsers.Contains(User))
                            {
                                Transform transform = SpawnPlayers.getSpawnPoint(User.networkPlayer, true);
                                userto(User, transform.position);
                                Reference.Tell(User.networkPlayer, msg);
                                counter++;
                            }
                        }
                        else
                        {
                            Transform transform = SpawnPlayers.getSpawnPoint(User.networkPlayer, true);
                            userto(User, transform.position);
                            Reference.Tell(User.networkPlayer, msg);
                            counter++;
                        }
                    }
                    return counter;

                }
                else
                {
                    if (user != null)
                    {
                        Transform transform = SpawnPlayers.getSpawnPoint(user.networkPlayer, true);
                        userto(user, transform.position);
                        Reference.Tell(user.networkPlayer, msg);
                        return 1;
                    }
                }

                return 0;

            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
                return 0;
            }

        }
        private static int skill(BetterNetworkUser user, int amount = 10000, bool all = false, string msg = "", BetterNetworkUser[] exclude = null)
        {

            try
            {

                BetterNetworkUser[] mapUsers = UserList.users.ToArray();
                List<BetterNetworkUser> exUsers = null;

                if ((exclude != null) && exclude.Length > 0)
                {
                    exUsers = new List<BetterNetworkUser>(exclude);
                }

                if (all)
                {
                    int counter = 0;
                    foreach (BetterNetworkUser User in mapUsers)
                    {
                        if ((exUsers != null) && exUsers.Count > 0)
                        {
                            if (!exUsers.Contains(User))
                            {
                                User.player.GetComponent<Skills>().learn(amount);
                                counter++;
                            }

                        }
                        else
                        {
                            User.player.GetComponent<Skills>().learn(amount);
                            counter++;
                        }

                    }
                    return counter;
                }
                else
                {
                    if (user != null)
                    {
                        user.player.GetComponent<Skills>().learn(amount);
                        return 1;
                    }

                }

                return 0;

            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
                return 0;
            }
        }
        private static int kill(BetterNetworkUser user, bool all = false, string msg = "", BetterNetworkUser[] exclude = null)
        {

            try
            {

                BetterNetworkUser[] mapUsers = UserList.users.ToArray();
                List<BetterNetworkUser> exUsers = null;

                if ((exclude != null) && exclude.Length > 0)
                {
                    exUsers = new List<BetterNetworkUser>(exclude);
                }

                if (all)
                {
                    int counter = 0;
                    foreach (BetterNetworkUser User in mapUsers)
                    {
                        if ((exUsers != null) && exUsers.Count > 0)
                        {
                            if (!exUsers.Contains(User))
                            {
                                User.player.gameObject.GetComponent<Life>().damage(10000, msg);
                                counter++;
                            }

                        }
                        else
                        {
                            User.player.gameObject.GetComponent<Life>().damage(10000, msg);
                            counter++;
                        }

                    }
                    return counter;
                }
                else
                {
                    if (user != null)
                    {
                        user.player.gameObject.GetComponent<Life>().damage(10000, msg);
                        return 1;
                    }

                }

                return 0;

            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
                return 0;
            }
        }
        private static int heal(BetterNetworkUser user, bool all = false, string msg = "", BetterNetworkUser[] exclude = null)
        {

            try
            {

                BetterNetworkUser[] mapUsers = UserList.users.ToArray();
                List<BetterNetworkUser> exUsers = null;

                if ((exclude != null) && exclude.Length > 0)
                {
                    exUsers = new List<BetterNetworkUser>(exclude);
                }

                if (all)
                {
                    int counter = 0;
                    foreach (BetterNetworkUser User in mapUsers)
                    {
                        if ((exUsers != null) && exUsers.Count > 0)
                        {
                            if (!exUsers.Contains(User))
                            {
                                User.player.gameObject.GetComponent<Life>().heal(100, true, true);
                                counter++;
                            }

                        }
                        else
                        {
                            User.player.gameObject.GetComponent<Life>().heal(100, true, true);
                            counter++;
                        }

                    }
                    return counter;
                }
                else
                {
                    if (user != null)
                    {
                        user.player.gameObject.GetComponent<Life>().heal(100, true, true);
                        return 1;
                    }

                }

                return 0;

            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
                return 0;
            }
        }

        private static int promote(BetterNetworkUser user)
        {
            int permission = UserList.getPermission(user.steamid);
            if (permission < PermissionLevel.Owner.ToInt())
            {
                File.IniWriteValue("PermissionLevels", user.steamid, (permission + 1).ToString());
                return (permission + 1);
            }
            return -1;
        }
        private static int demote(BetterNetworkUser user)
        {
            int permission = UserList.getPermission(user.steamid);
            if (permission > PermissionLevel.All.ToInt())
            {
                File.IniWriteValue("PermissionLevels", user.steamid, (permission - 1).ToString());
                return (permission - 1);
            }
            return -1;
        }

        private static void userto(BetterNetworkUser user, Vector3 target)
        {
            try
            {
                user.position = target;
                user.player.gameObject.GetComponent<Life>().networkView.RPC("tellStatePosition", RPCMode.All, new object[] { target, user.rotation });
                user.player.gameObject.GetComponent<NetworkInterpolation>().tellStatePosition_Pizza(target, user.rotation);

                Network.SetReceivingEnabled(user.networkPlayer, 0, false);

                System.Threading.Timer timer;
                timer = new System.Threading.Timer(obj =>
                {
                    Network.SetReceivingEnabled(user.networkPlayer, 0, true);
                }, null, 2000, System.Threading.Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
            }
        }
        private static void userto(BetterNetworkUser user, Vector3 targetposition, Quaternion targetrotation)
        {
            try
            {
                user.position = targetposition;
                user.player.gameObject.GetComponent<Life>().networkView.RPC("tellStatePosition", RPCMode.All, new object[] { targetposition, targetrotation });
                user.player.gameObject.GetComponent<NetworkInterpolation>().tellStatePosition_Pizza(targetposition, targetrotation);

                Network.SetReceivingEnabled(user.networkPlayer, 0, false);

                System.Threading.Timer timer;
                timer = new System.Threading.Timer(obj =>
                {
                    Network.SetReceivingEnabled(user.networkPlayer, 0, true);
                }, null, 2000, System.Threading.Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
            }
        }

    }
}
