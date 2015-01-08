using CommandHandler;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Unturned
{
    internal class DeathMessages : Module
    {

        #region TOP: global variables are initialized here

        internal FieldInfo[] lifefields = typeof(Life).GetFields();
        internal List<Player> announcedDeadPeople = new List<Player>();
        internal float lastupdate = 0f;

        internal static bool UseDeathMessage = true;

        #endregion

        internal override void Load()
        {

            if (String.IsNullOrEmpty(Configs.File.IniReadValue("Modules", "DeathMessages")))
            {
                Configs.File.IniWriteValue("Modules", "DeathMessages", "true");
            }

            DeathMessages.UseDeathMessage = Boolean.Parse(Configs.File.IniReadValue("Modules", "DeathMessages"));

        }
        internal override void Refresh()
        {
            if (DeathMessages.UseDeathMessage)
            {
                if (Time.realtimeSinceStartup - this.lastupdate >= 3f)
                {
                    Player[] players = UnityEngine.Object.FindObjectsOfType<Player>();
                    this.lastupdate = Time.realtimeSinceStartup;
                }
                CheckDeadPlayers();
            }
        }

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();

            return _return;
        }
        internal override String GetHelp()
        {
            return null;
        }

        #region Commands

        #endregion

        #region Private calls

        internal void CheckDeadPlayers()
        {
            try
            {

                foreach (Player player in UnityEngine.Object.FindObjectsOfType<Player>())
                {
                    if (!(player == null) && !(player.GetComponent<Life>() == null))
                    {
                        Life component = player.GetComponent<Life>();
                        if ((bool)this.lifefields[2].GetValue(component))
                        {
                            if (!this.announcedDeadPeople.Contains(player))
                            {
                                this.announcedDeadPeople.Add(player);
                                string text = (string)this.lifefields[3].GetValue(component);
                                if (text.StartsWith("You were "))
                                {
                                    text = " was " + text.Substring(9);
                                }
                                else
                                {
                                    if (text.StartsWith("You "))
                                    {
                                        text = text.Substring(3);
                                    }
                                }
                                if (text.Contains("yourself"))
                                {
                                    text = text.Substring(0, text.IndexOf("yourself")) + "himself" + text.Substring(text.IndexOf("yourself") + 8);
                                }
                                if (text.Contains("your"))
                                {
                                    text = text.Substring(0, text.IndexOf("your")) + "his" + text.Substring(text.IndexOf("your") + 4);
                                }
                                NetworkChat.sendAlert(player.name + text);
                            }
                        }
                        else
                        {
                            if (this.announcedDeadPeople.Contains(player))
                            {
                                this.announcedDeadPeople.Remove(player);
                            }
                        }
                    }

                }
            }
            catch
            {
            }
        }

        #endregion

    }
}
