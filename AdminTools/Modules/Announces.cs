using CommandHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace Unturned
{
    internal class Announces : Module
    {

        #region TOP: global variables are initialized here

        private static System.Timers.Timer Timer;
        internal static int Interval = 600;

        internal static bool UseAnnounces = false;

        internal static Stack<String> Messages;

        private static string fileSource = System.IO.Path.Combine(AdminTools.AdminPath, "announces.txt");

        #endregion

        internal override void Load()
        {
            if (String.IsNullOrEmpty(Configs.File.IniReadValue("Modules", "Announces")))
            {
                Configs.File.IniWriteValue("Modules", "Announces", "true");
                Configs.File.IniWriteValue("Timers", "Announces", "600");
            }

            Announces.UseAnnounces = Boolean.Parse(Configs.File.IniReadValue("Modules", "Announces"));
            Announces.Interval = Int32.Parse(Configs.File.IniReadValue("Timers", "Announces"));

            if (Announces.UseAnnounces)
            {
                if (Timer != null)
                {
                    Timer.Enabled = false;
                    Timer.Elapsed -= announceTimer_Elapsed;
                    Timer.Dispose();
                    Timer = null;
                }

                if (!File.Exists(fileSource)) { Create(); }

                string[] announces = System.IO.File.ReadAllLines(fileSource);
                Messages = new Stack<string>(announces);

                if (Messages.Count > 0)
                {
                    Timer = new System.Timers.Timer(Announces.Interval * 1000);
                    Timer.Elapsed += announceTimer_Elapsed;
                    Timer.Enabled = true;
                }
                else
                {
                    Messages = null;
                }
            }

        }
        internal override void Create()
        {
            System.IO.StreamWriter file = new StreamWriter(fileSource, true);
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
        internal override void Clear()
        {
            Announces.Messages = null;
        }

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Announce, "repeat", "say", "announce")); // Use /say <text>
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), SetDelay, "setannouncedelay", "adelay"));
            return _return;
        }
        internal override String GetHelp()
        {
            return null;
        }

        #region Commands

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

        #endregion

        #region Private calls

        private void announceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Next();

            if (Messages.Count == 0) { this.Load(); }

        }

        private static void Next()
        {
            string message = Messages.Pop();
            if (message.Equals(":"))
            {
                return;
            }
            else
            {
                NetworkChat.sendAlert(message);
            }

        }
        private static void SetDelay(int seconds)
        {
            if (Timer != null) // && announceTimer.Enabled)
            {
                Interval = seconds;
                Timer.Stop();
                Timer.Interval = seconds * 1000;
                Timer.Start();
            }
        }

        #endregion

    }
}
