using CommandHandler;
using System;
//using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Unturned
{
    public class AdminTools : MonoBehaviour
    {

        #region TOP: global variables are initialized here

        internal const string ModsPath = "Unturned_Data/Managed/mods";
        internal const string AdminPath = ModsPath + "/Admin";
                
        internal static List<Module> Modules = new List<Module>();
        internal static int LoadedModules;

        #endregion

        public void Start()
        {

            Configs.Load();

            try
            {

                foreach (Type type in Assembly.GetAssembly(typeof(Module)).GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Module)))
                    {
                        Modules.Add((Module)Activator.CreateInstance(type, null));
                    }
                }

                foreach (Module item in Modules)
                {
                    foreach (Command citem in item.GetCommands())
                    {
                        CommandList.add(citem);
                    }
                    item.Load();
                    LoadedModules++;
                }

            }
            catch (Exception ex)
            {
                Shared.Log(ex.ToString());
            }

            CommandList.add(PermissionLevel.All.ToInt(), PrintHelp, "help", "?");

            Shared.Log(Strings.Get("LOG", "ServerStarted") + " " + String.Format(Strings.Get("GUI", "LoadedModules"), LoadedModules));

        }

        internal static void PrintHelp(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {

                Module _mod = null;
                foreach (Module item in Modules)
                {
                    if (item.GetType().ToString().ToLower().EndsWith(args.Parameters[0].ToLower()))
                    {
                        _mod = item;
                        break;
                    }
                }

                //Module _mod = Modules.FirstOrDefault(s => s.GetType().ToString().ToLower().EndsWith(args.Parameters[0].ToLower()));
                if (_mod != null)
                {
                    string[] result = _mod.GetHelp().Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in result)
                    {
                        Reference.Tell(args.sender.networkPlayer, item);
                    }
                }
                else
                {
                    Reference.Tell(args.sender.networkPlayer, "Module was not found.");
                }
            }
            else
            {
                List<String> _mods = new List<String>();
                foreach (Module item in Modules)
                {
                    String _name = item.GetType().ToString().ToLower();

                    String[] parts = _name.Split('.');
                    _mods.Add(parts[parts.Length - 1]);
                    break;

                }
                //string[] _mod = Modules.Select(s => s.GetType().ToString().Split('.').Last().ToLower()).ToArray();
                Reference.Tell(args.sender.networkPlayer, string.Format("Use [help,?] <[{0}]>", String.Join(", ", _mods.ToArray())));
            }
        }

        public void Update()
        {

            foreach (Module item in Modules)
            {
                item.Refresh();
            }

        }

        public void OnGUI()
        {

            if (Configs.Console)
            {

                List<string> console = new List<string>();
                console.Add(" ");
                console.Add(Strings.Get("GUI", "WelcomeMessage"));
                if (Configs.Developer)
                {
                    console.Add(String.Format(Strings.Get("GUI", "RunningVersion"), System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()));
                    console.Add(String.Format(Strings.Get("GUI", "LoadedModules"), Modules.Count));
                }

                console.Add(" ");

                foreach (BetterNetworkUser item in UserList.users)
                {
                    if (!string.IsNullOrEmpty(item.networkPlayer.ipAddress))
                    {
                        console.Add(string.Format("{0}    {1}     {2}     {3}",
                        item.steamid, item.reputation, item.networkPlayer.ipAddress, item.name));
                    }
                }

                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height / 2));

                foreach (string item in console)
                {
                    GUILayout.Label(item);
                }
                GUILayout.EndArea();

                GUILayout.BeginArea(new Rect(0, Screen.height / 3, Screen.width, Screen.height / 3));

                foreach (Module item in Modules)
                {
                    item.Print();
                }

                GUILayout.EndArea();

            }


        }

    }

}