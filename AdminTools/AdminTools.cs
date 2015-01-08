using CommandHandler;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Unturned
{
    public class AdminTools : MonoBehaviour
    {

        #region TOP: global variables are initialized here

        internal const string AdminPath = "Unturned_Data/Managed/mods/Admin";
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

            Shared.Log(Strings.Get("LOG", "ServerStarted") + " " + String.Format(Strings.Get("GUI", "LoadedModules"), LoadedModules));

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

            List<string> console = new List<string>();
            console.Add(" ");
            console.Add(Strings.Get("GUI","WelcomeMessage"));
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
            foreach (Module item in Modules)
            {
                item.Print();
            }

            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

            foreach (string item in console)
            {
                GUILayout.Label(item);
            }
            GUILayout.EndArea();


        }

    }

}