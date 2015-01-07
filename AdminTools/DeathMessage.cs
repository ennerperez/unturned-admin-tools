using CommandHandler;
using Ini;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Unturned
{
    public class DeathMessage : MonoBehaviour
    {
        private FieldInfo[] lifefields = typeof(Life).GetFields();
        private List<Player> announcedDeadPeople = new List<Player>();
        //private Player[] players = UnityEngine.Object.FindObjectsOfType<Player>();
        private float lastupdate = 0f;

        public void Start()
        {
        }

        private void CheckDeadPlayers()
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
        public void Update()
        {
            if (Time.realtimeSinceStartup - this.lastupdate >= 3f)
            {
                Player[] players = UnityEngine.Object.FindObjectsOfType<Player>();
                this.lastupdate = Time.realtimeSinceStartup;
            }
            this.CheckDeadPlayers();
        }
        public void OnGui()
        {
        }
    }
}
