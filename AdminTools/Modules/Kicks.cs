using CommandHandler;
using System;
using System.Collections.Generic;

namespace Unturned
{
    internal class Kicks : Module
    {

        #region TOP: global variables are initialized here

        #endregion

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Kick, "kick", "k"));
            return _return;
        }
        internal override String GetHelp()
        {
            return null;
        }

        #region Commands

        internal static void Kick(CommandArgs args)
        {
            String name = args.ParametersAsString;
            kick(name, "You were kicked off the server");
        }

        internal static void kick(String name, String reason)
        {
            BetterNetworkUser user = UserList.getUserFromName(name);
            NetworkTools.kick(user.networkPlayer, reason);
        }

        #endregion

    }
}
