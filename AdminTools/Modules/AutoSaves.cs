using CommandHandler;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Timers;
using UnityEngine;

namespace Unturned
{
    internal class AutoSaves : Module
    {

        #region TOP: global variables are initialized here

        private static System.Timers.Timer Timer;

        internal static int Interval = 600;
        internal static bool UseAutoSave = false;

        #endregion

        internal override void Load()
        {

            if (String.IsNullOrEmpty(Configs.File.IniReadValue("Modules", "AutoSaves")))
            {
                Configs.File.IniWriteValue("Modules", "AutoSaves", "true");
                Configs.File.IniWriteValue("Timers", "AutoSaves", "600");
            }

            AutoSaves.UseAutoSave = Boolean.Parse(Configs.File.IniReadValue("Modules", "AutoSaves"));
            AutoSaves.Interval = Int32.Parse(Configs.File.IniReadValue("Timers", "AutoSaves"));

            if (AutoSaves.UseAutoSave)
            {
                Timer = new System.Timers.Timer(AutoSaves.Interval * 1000);
                Timer.Elapsed += autoSaveTimer_Elapsed;
                Timer.Enabled = true;
            }

        }
        internal override void Refresh()
        {
            return;
        }

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), SaveCommand, "save", "saveserver"));
            return _return;
        }
        internal override String GetHelp()
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
            NetworkChat.sendAlert(Strings.Get("MOD","AutoSavesSaving"));
            try
            {
                NetworkTools.save();
            }
            catch
            {
                Shared.Log(Strings.Get("MOD", "AutoSavesFailed"));
                Shared.Log(Strings.Get("MOD", "AutoSavesSavingTryManual"));
                while (true)
                {
                    try
                    {
                        saveStructuresManually();
                        Shared.Log(Strings.Get("MOD", "AutoSavesSavedStructures"));
                        break;
                    }
                    catch (Exception ex)
                    {
                        Shared.Log(Strings.Get("MOD", "AutoSavesError") + ": " + ex.Message);
                        Shared.Log(Strings.Get("MOD", "AutoSavesRetrying"));
                    }
                }
                while (true)
                {
                    try
                    {
                        SpawnVehicles.save();
                        Shared.Log(Strings.Get("MOD", "AutoSavesSavedVehicles"));
                        break;
                    }
                    catch (Exception ex)
                    {
                        Shared.Log(Strings.Get("MOD", "AutoSavesError") + ": " + ex.Message);
                        Shared.Log(Strings.Get("MOD", "AutoSavesRetrying"));
                    }
                }
                while (true)
                {
                    try
                    {
                        PlayerPrefs.Save();
                        Shared.Log(Strings.Get("MOD", "AutoSavesSavedPlayerPrefs"));
                        break;
                    }
                    catch (Exception ex)
                    {
                        Shared.Log(Strings.Get("MOD", "AutoSavesError") + ": " + ex.Message);
                        Shared.Log(Strings.Get("MOD", "AutoSavesRetrying"));
                    }
                }
            }
            Shared.Log(Strings.Get("MOD", "AutoSavesSaved"));
            NetworkChat.sendAlert(Strings.Get("MOD", "AutoSavesDone"));
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
                Shared.Log(Strings.Get("MOD", "AutoSavesStuckStructures"));
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
							String.Format(Strings.Get("MOD","AutoSavesFailedStructures"), num3, num2, num)
						}));
                        Shared.Log(Strings.Get("MOD", "AutoSavesCancelling"));
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
            Shared.Log(String.Format(Strings.Get("MOD","AutoSavesSavedManual"),list.Count,num));
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
