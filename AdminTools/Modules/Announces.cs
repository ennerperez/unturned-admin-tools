using CommandHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace Unturned
{
    public static class Announces
    {
        #region TOP: global variables are initialized here

        private static Timer AnnounceTimer;
        internal static int AnnouncesInterval = 600;
        internal static Stack<String> AnnouncesMessages;

        #endregion

        internal static void Load()
        {

            if (AnnounceTimer != null) // || announceTimer.Enabled)
            {
                AnnounceTimer.Enabled = false;
                AnnounceTimer.Elapsed -= announceTimer_Elapsed;
                AnnounceTimer.Dispose();
                AnnounceTimer = null;
            }

            string fileSource = System.IO.Path.Combine(AdminTools.Path, "announces.txt");
            if (!File.Exists(fileSource))
            {
                Create();
            }

            string[] announces = System.IO.File.ReadAllLines(fileSource);
            AnnouncesMessages = new Stack<string>(announces);

            if (AnnouncesMessages.Count > 0)
            {
                AnnounceTimer = new Timer(AnnouncesInterval * 1000);
                AnnounceTimer.Elapsed += announceTimer_Elapsed;
                AnnounceTimer.Enabled = true;
            }
            else
            {
                AnnouncesMessages = null;
            }

        }

        internal static void GetCommands()
        {
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Announce, "repeat", "say", "announce")); // Use /say <text>
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), SetDelay, "setannouncedelay", "adelay"));
        }

        internal static void Announce(CommandArgs args)
        {
            string concat = args.ParametersAsString;
            NetworkChat.sendAlert(concat);
        }
        internal static void SetDelay(CommandArgs args)
        {
            String seconds = args.Parameters[0];
            SetDelay(Convert.ToInt32(seconds));
        }

        private static void announceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Next();
        }

        private static void Create()
        {
            System.IO.StreamWriter file = new StreamWriter(System.IO.Path.Combine(AdminTools.Path, "announces.txt"), true);
            file.WriteLine("This line will be announced 10 minutes after injecting (or whatever you change the interval to)");
            file.WriteLine("This line will be announced at the same time");
            file.WriteLine(":");
            file.WriteLine("This line will be announced 20 minutes after injecting  (2x interval)");
            file.WriteLine(":");
            file.WriteLine(":");
            file.WriteLine("This line will be announced 40 minutes after injecting  (4x interval)");
            file.WriteLine("And so forth.. then it will go back to the 1st line      (4x interval)");
            file.Close();
        }
        private static void Next()
        {
            string message = AnnouncesMessages.Pop();
            if (message.Equals(":"))
            {
                return;
            }
            else
            {
                NetworkChat.sendAlert(message);
            }

            if (AnnouncesMessages.Count == 0)
            {
                Load();
            }
        }
        private static void SetDelay(int seconds)
        {
            if (AnnounceTimer != null) // && announceTimer.Enabled)
            {
                AnnouncesInterval = seconds;
                AnnounceTimer.Stop();
                AnnounceTimer.Interval = seconds * 1000;
                AnnounceTimer.Start();
            }
        }

    }
}
