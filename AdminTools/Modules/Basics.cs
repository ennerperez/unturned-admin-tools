using CommandHandler;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unturned
{
    internal class Basics : Module
    {

        #region TOP: global variables are initialized here

        #endregion

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.All.ToInt(), ClearChat, "clear", "cls"));
            _return.Add(new Command(PermissionLevel.All.ToInt(), OnlinePlayers, "online", "players", "who"));
            _return.Add(new Command(PermissionLevel.All.ToInt(), GameTime, "time"));
            _return.Add(new Command(PermissionLevel.All.ToInt(), RealTime, "now"));
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), Restart, "restart"));
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), About, "about"));
            return _return;
        }
        internal override String GetHelp()
        {
            return null;
        }

        #region Commands

        internal static void ClearChat(CommandArgs args)
        {
            for (int i = 0; i < 5; i++)
            {
                Reference.Tell(args.sender.networkPlayer, " ");
            }
        }
        internal static void OnlinePlayers(CommandArgs args)
        {
            NetworkChat.sendAlert(String.Format("There are {0} players online.", UserList.NetworkUsers.Count));
        }
        internal static void GameTime(CommandArgs args)
        {
            NetworkChat.sendAlert(String.Format("The time is {0}", Sun.getTime()));
        }
        internal static void RealTime(CommandArgs args)
        {
            NetworkChat.sendAlert(String.Format("The real time is {0}", DateTime.Now.ToShortTimeString()));
        }
        internal static void Restart(CommandArgs args)
        {
            NetworkChat.sendAlert("The server is restarting...");
            System.Threading.Thread.Sleep(3000);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        internal static void About(CommandArgs args)
        {
            String text1 = String.Format("{0} v{1}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Reference.Tell(args.sender.networkPlayer, text1);
        }

        #endregion

    }
}
