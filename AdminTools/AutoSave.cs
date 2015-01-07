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

        #region TOP: global variables are initialized here

        private static System.Timers.Timer Timer;
        internal static int Interval = 600;

        internal static bool UseAutoSave = false;
        
        #endregion

        internal void Load()
        {

            Configs.Load();

            if (String.IsNullOrEmpty(Configs.File.IniReadValue("Config", "UseAutoSave"))) 
            {
                Configs.File.IniWriteValue("Config", "UseAutoSave", "true");
                Configs.File.IniWriteValue("Timers", "AutoSave", "600");
            }

            AutoSave.UseAutoSave = Boolean.Parse(Configs.File.IniReadValue("Config", "UseAutoSave"));
            AutoSave.Interval = Int32.Parse(Configs.File.IniReadValue("Timers", "AutoSave"));                       

            if (AutoSave.UseAutoSave)
            {
                Timer = new System.Timers.Timer(AutoSave.Interval * 1000);
                Timer.Elapsed += autoSaveTimer_Elapsed;
                Timer.Enabled = true;
            }
          
        }
        internal static void Create()
        {
        }

        public void Start()
        {

            Load();

            foreach (Command citem in GetCommands())
            {
                CommandList.add(citem);
            }        
                    
        }

        internal IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), SaveCommand, "save", "saveserver")); 
            return _return;
        }
        internal String GetHelp()
        {
            return null;
        }

        #region Commands

        internal static void SaveCommand(CommandArgs args)
        {
            ThreadPool.QueueUserWorkItem(delegate(object param0)
            {
                SaveServer();
            }, null);
        }

        #endregion

        #region Private calls

        private void autoSaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(delegate(object param0)
            {
                SaveServer();
            }, null);
        }

        internal static void SaveServer()
        {
            NetworkChat.sendAlert("Saving world...");
            try
            {
                NetworkTools.save();
            }
            catch
            {
                Shared.Log("NetworkTools.save() failed to save correctly: ");
                Shared.Log("Will try to continue saving structures and vehicles manually...");
                while (true)
                {
                    try
                    {
                        saveStructuresManually();
                        Shared.Log("Structures saved :D");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Shared.Log("An error happened while saving structures :(  ; " + ex.Message);
                        Shared.Log("Retrying...");
                    }
                }
                while (true)
                {
                    try
                    {
                        SpawnVehicles.save();
                        Shared.Log("Vehicles saved :D");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Shared.Log("An error happened while saving vehicles :(  ; " + ex.Message);
                        Shared.Log("Retrying...");
                    }
                }
                while (true)
                {
                    try
                    {
                        PlayerPrefs.Save();
                        Shared.Log("PlayerPrefs saved :D");
                        break;
                    }
                    catch (Exception ex)
                    {
                        Shared.Log("An error happened while saving PlayerPrefs :(  ; " + ex.Message);
                        Shared.Log("Retrying...");
                    }
                }
            }
            DateTime now = DateTime.Now;
            string text = now.ToString("yyyy-MM-dd HH:mm:ss");
            Shared.Log(now + " : Saved successfully ============================================");
            NetworkChat.sendAlert("Done.");
        }
        internal static void saveStructuresManually()
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
                Shared.Log("Stuck at getting structure list!");
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
                        Shared.Log(string.Concat(new object[]
						{
							"Could not get field number ",
							num3,
							" of structure number ",
							num2,
							". Failed to get ",
							num,
							" structures"
						}));
                        Shared.Log("Cancelling :(");
                        return;
                    }
                    num2++;
                }
            }
            string text2 = sneakString(text);
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
				sneakString(text),
				"_"
			}));
            Shared.Log(string.Concat(new object[]
			{
				"Manually saved ",
				list.Count,
				" structures. (",
				num,
				" structures failed to save)"
			}));
        }
        internal static string sneakString(string structurestring)
        {
            char[] array = structurestring.ToCharArray();
            string text = string.Empty;
            for (int i = array.Length - 1; i >= 0; i--)
            {
                text += array[i] + '\u2000';
            }
            return text;
        }      

        #endregion

        

    }
}
