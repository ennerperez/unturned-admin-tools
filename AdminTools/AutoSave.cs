using CommandHandler;
using Ini;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Timers;
using UnityEngine;

namespace Unturned
{
    public class AutoSave : MonoBehaviour
    {
        public int saveDelayInSeconds = 600;
        public string messageBeforeSave = "Saving world...";
        public string messageAfterSave = "Done!";
        private System.Timers.Timer saveTimer;
        public void Start()
        {
            Directory.CreateDirectory("Unturned_Data/Managed/mods/AutoSave");
            if (!File.Exists("Unturned_Data/Managed/mods/AutoSave/config.ini"))
            {
                StreamWriter streamWriter = new StreamWriter("Unturned_Data/Managed/mods/AutoSave/config.ini", true);
                streamWriter.WriteLine("[Config]");
                streamWriter.WriteLine("Save delay in seconds=600");
                streamWriter.WriteLine("Announcement before saving=Saving world...");
                streamWriter.WriteLine("Announcement after saving=Done!");
                streamWriter.Close();
            }
            string[] array = File.ReadAllLines("Unturned_Data/Managed/mods/AutoSave/config.ini");
            this.saveDelayInSeconds = Convert.ToInt32(array[1].Substring(22));
            this.messageBeforeSave = array[2].Substring(27);
            this.messageAfterSave = array[3].Substring(26);
            this.saveTimer = new System.Timers.Timer((double)(this.saveDelayInSeconds * 1000));
            this.saveTimer.Elapsed += new ElapsedEventHandler(this.SaveServer);
            this.saveTimer.Enabled = true;
            CommandList.add(new Command(new CommandDelegate(this.SaveCommand), new string[]
			{
				"save",
				"saveserver"
			}));
        }
        private void SaveCommand(CommandArgs args)
        {
            ThreadPool.QueueUserWorkItem(delegate(object param0)
            {
                this.SaveServer();
            }, null);
        }
        private void SaveServer()
        {
            NetworkChat.sendAlert(this.messageBeforeSave);
            try
            {
                NetworkTools.save();
            }
            catch
            {
                this.Log("NetworkTools.save() failed to save correctly: ");
                this.Log("Will try to continue saving structures and vehicles manually...");
                while (true)
                {
                    try
                    {
                        this.saveStructuresManually();
                        this.Log("Structures saved :D");
                        break;
                    }
                    catch (Exception ex)
                    {
                        this.Log("An error happened while saving structures :(  ; " + ex.Message);
                        this.Log("Retrying...");
                    }
                }
                while (true)
                {
                    try
                    {
                        SpawnVehicles.save();
                        this.Log("Vehicles saved :D");
                        break;
                    }
                    catch (Exception ex)
                    {
                        this.Log("An error happened while saving vehicles :(  ; " + ex.Message);
                        this.Log("Retrying...");
                    }
                }
                while (true)
                {
                    try
                    {
                        PlayerPrefs.Save();
                        this.Log("PlayerPrefs saved :D");
                        break;
                    }
                    catch (Exception ex)
                    {
                        this.Log("An error happened while saving PlayerPrefs :(  ; " + ex.Message);
                        this.Log("Retrying...");
                    }
                }
            }
            DateTime now = DateTime.Now;
            string text = now.ToString("yyyy-MM-dd HH:mm:ss");
            this.Log(now + " : Saved successfully ============================================");
            NetworkChat.sendAlert(this.messageAfterSave);
        }
        private void saveStructuresManually()
        {
            FieldInfo[] fields = typeof(SpawnStructures).GetFields();
            FieldInfo[] fields2 = typeof(ServerStructure).GetFields();
            List<ServerStructure> list = new List<ServerStructure>();
            try
            {
                list = (List<ServerStructure>)fields[1].GetValue(null);
            }
            catch
            {
                this.Log("Stuck at getting structure list!");
            }
            string text = string.Empty;
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            foreach (ServerStructure current in list)
            {
                if (current == null)
                {
                    num++;
                }
                else
                {
                    try
                    {
                        text = text + fields2[0].GetValue(current) + ":";
                        num3++;
                        text = text + fields2[1].GetValue(current) + ":";
                        num3++;
                        text = text + fields2[2].GetValue(current) + ":";
                        num3++;
                        text = text + Mathf.Floor(((Vector3)fields2[3].GetValue(current)).x * 100f) / 100f + ":";
                        num3++;
                        text = text + Mathf.Floor(((Vector3)fields2[3].GetValue(current)).y * 100f) / 100f + ":";
                        num3++;
                        text = text + Mathf.Floor(((Vector3)fields2[3].GetValue(current)).z * 100f) / 100f + ":";
                        num3++;
                        text = text + fields2[4].GetValue(current) + ":;";
                    }
                    catch
                    {
                        this.Log(string.Concat(new object[]
						{
							"Could not get field number ",
							num3,
							" of structure number ",
							num2,
							". Failed to get ",
							num,
							" structures"
						}));
                        this.Log("Cancelling :(");
                        return;
                    }
                    num2++;
                }
            }
            string text2 = this.sneakString(text);
            int num4 = (int)typeof(ServerSettings).GetFields()[0].GetValue(null);
            int num5 = (int)typeof(MasterSettings).GetFields()[0].GetValue(null);
            int num6 = (int)typeof(Savedata).GetFields()[6].GetValue(null);
            int[] array = (int[])typeof(Maps).GetFields()[1].GetValue(null);
            PlayerPrefs.SetString("structures_" + num4, string.Concat(new object[]
			{
				num5,
				"_",
				num6,
				"_",
				array[num4],
				"_",
				this.sneakString(text),
				"_"
			}));
            this.Log(string.Concat(new object[]
			{
				"Manually saved ",
				list.Count,
				" structures. (",
				num,
				" structures failed to save)"
			}));
        }
        private string sneakString(string structurestring)
        {
            char[] array = structurestring.ToCharArray();
            string text = string.Empty;
            for (int i = array.Length - 1; i >= 0; i--)
            {
                text += array[i] + '\u2000';
            }
            return text;
        }
        private void SaveServer(object sender, ElapsedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(delegate(object param0)
            {
                this.SaveServer();
            }, null);
        }
        private void Log(string p)
        {
            StreamWriter streamWriter = new StreamWriter("Unturned_Data/Managed/mods/AutoSave/AutoSave_Log.txt", true);
            streamWriter.WriteLine(p);
            streamWriter.Close();
        }
    }
}
