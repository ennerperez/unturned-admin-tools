using CommandHandler;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Unturned
{
    public static class Kits
    {

        #region TOP: global variables are initialized here

        internal static Dictionary<String, Dictionary<string, int[]>> PlayerKits;
        internal static bool UsePlayerKits = false;

        #endregion
        
        internal static void Load()
        {
            if (UsePlayerKits)
            {
                string fileSource = System.IO.Path.Combine(AdminTools.Path, "kits.txt");
                if (!File.Exists(fileSource))
                {
                    create();
                }

                PlayerKits = new Dictionary<String, Dictionary<string, int[]>>();
                string[] kits = System.IO.File.ReadAllLines(fileSource);
                foreach (string item in kits)
                {
                    try
                    {
                        string[] values = item.Split(':');
                        String id = values[0];
                        String name = values[1];
                        String items = values[2];
                        List<int> itemsid = new List<int>();
                        foreach (string itemid in items.Split(','))
                        {
                            itemsid.Add(int.Parse(itemid));
                        }
                        string[] key = { id, name };
                        if (!PlayerKits.ContainsKey(id))
                        {
                            PlayerKits.Add(id, new Dictionary<string, int[]>());
                        }
                        PlayerKits[id].Add(name, itemsid.ToArray());
                    }
                    catch (Exception ex)
                    {
                        AdminTools.Log(ex);
                    }
                }
            }
        }
        
        internal static void GetCommands()
        {
            CommandList.add(new Command(PermissionLevel.Owner.ToInt(), List, "getkits", "ks", "kits"));
            CommandList.add(new Command(PermissionLevel.Admin.ToInt(), Get, "kit", "k")); // Use /k <kitname>
            CommandList.add(new Command(PermissionLevel.Admin.ToInt(), AddTo, "addkit", "ka")); // Use /ka <kitname> <[itemid]>
            CommandList.add(new Command(PermissionLevel.Owner.ToInt(), Set, "setkit", "ks")); // Use /ks <kitname> <[itemid1]> <[itemid2]> <[itemid3]>
        }
        
        internal static void List(CommandArgs args)
        {
            if (UsePlayerKits)
            {
                string userKits = "";
                List<string> userKitsList = new List<string>();

                foreach (KeyValuePair<string, int[]> item in PlayerKits[args.sender.steamid])
                {
                    userKitsList.Add(item.Key);
                }

                if (userKitsList.Count > 0)
                {
                    userKits = string.Join(", ", userKitsList.ToArray());
                    Reference.Tell(args.sender.networkPlayer, String.Format("You can use those kits: {0}. Use /kit <name>", userKits));
                }
                else
                {
                    Reference.Tell(args.sender.networkPlayer, "You don't have kits set yet.");
                }
            }
        }
        internal static void Set(CommandArgs args)
        {
            if (UsePlayerKits)
            {
                List<int> itemsID = new List<int>();
                for (int i = 1; i <= args.Parameters.Count - 1; i++)
                {
                    itemsID.Add(int.Parse(args.Parameters[i]));
                }
                string kitname = args.Parameters[0].ToString();
                set(args.sender.steamid, kitname, itemsID.ToArray());
                if (itemsID.Count > 0)
                {
                    Reference.Tell(args.sender.networkPlayer, String.Format("Kit '{0}' was set.", kitname));
                }
                else
                {
                    Reference.Tell(args.sender.networkPlayer, String.Format("Kit '{0}' was deleted.", kitname));
                }

            }
        }
        internal static void AddTo(CommandArgs args)
        {
            if (UsePlayerKits)
            {
                string kitname = args.Parameters[0].ToString();
                int itemid = int.Parse(args.Parameters[1]);
                addTo(args.sender.steamid, kitname, itemid);

                Reference.Tell(args.sender.networkPlayer, String.Format("'{0}' was added to kit '{1}'.", ItemName.getName(itemid), kitname));

            }
        }
        internal static void Get(CommandArgs args)
        {
            int[] itemids = null;

            if (UsePlayerKits == true)
            {
                if (args.Parameters.Count > 0)
                {
                    string kitname = args.Parameters[0];
                    string[] key = { args.sender.steamid, kitname };

                    if (PlayerKits[args.sender.steamid].ContainsKey(kitname))
                    {
                        itemids = PlayerKits[args.sender.steamid][kitname];
                    }
                }
            }

            if (itemids == null && args.Parameters.Count == 0) { itemids = new int[] { 0x7d4, 0x1b60, 0x2ee0, 0x232c, 0x2711, 0x2afb, 0x465e, 0x465e, 0x465e, 0x465e, 0x465e, 0x465e, 0x465e, 0xfb1, 0x1399, 11, 0x32c8, 0x32c8, 0x36c6, 0x36c6, 0x1f4f, 0x1f4d, 0xbba }; }

            if (itemids != null)
            {
                Vector3 location = args.sender.position;
                Inventory inventory = args.sender.player.gameObject.GetComponent<Inventory>();

                //TODO SORT DESC
                List<int> sortItems = new List<int>();
                sortItems.AddRange(itemids);
                sortItems.Sort();

                // Finding bags
                Clothes cloth = args.sender.player.gameObject.GetComponent<Clothes>();
                int maxitem = 0;
                foreach (int item in sortItems)
                {
                    if (item >= 2000 & item <= 2006)
                    {
                        if (item > maxitem) { maxitem = item; }
                    }

                    if (maxitem > 0)
                    {
                        cloth.changeBackpack(maxitem);
                        cloth.saveAllClothing();
                    }
                }

                foreach (int item in sortItems)
                {
                    if (item < 2000 || item > 2006)
                    {
                        inventory.tryAddItem(item, 1);
                    }

                }

                cloth.loadAllClothing();

            }
            else
            {
                Reference.Tell(args.sender.networkPlayer, "Kit was not found.");
            }

        }

        private static void create()
        {
            System.IO.StreamWriter file = new StreamWriter(System.IO.Path.Combine(AdminTools.Path, "kits.txt"), true);
            file.Close();
        }
        private static void save()
        {
            string fileSource = System.IO.Path.Combine(AdminTools.Path, "kits.txt");
            string[] lines = System.IO.File.ReadAllLines(fileSource);
            File.Delete(fileSource);

            System.IO.StreamWriter file = new StreamWriter(fileSource, true);
            foreach (KeyValuePair<String, Dictionary<string, int[]>> item in PlayerKits)
            {
                string steamID = item.Key;
                foreach (KeyValuePair<string, int[]> kit in item.Value)
                {
                    string kitname = kit.Key;
                    List<string> itemsID = new List<string>();
                    foreach (int kititem in kit.Value)
                    {
                        itemsID.Add(kititem.ToString());
                    }

                    file.WriteLine(String.Format("{0}:{1}:{2}", steamID, kitname, string.Join(",", itemsID.ToArray())));
                }

            }

            file.Close();

        }

        private static void addTo(string steamID, string kitname, int itemID)
        {
            if (itemID <= 0) { return; }

            if (!PlayerKits.ContainsKey(steamID))
            {
                PlayerKits.Add(steamID, new Dictionary<string, int[]>());
            }

            if (!PlayerKits[steamID].ContainsKey(kitname))
            {
                PlayerKits[steamID].Add(kitname, new int[] { itemID });
            }

            List<int> itemids = new List<int>();
            itemids.AddRange(PlayerKits[steamID][kitname]);
            itemids.Add(itemID);
            PlayerKits[steamID][kitname] = itemids.ToArray();

            save();

        }
        private static void set(string steamID, string kitname, int[] itemsID)
        {
            if (!PlayerKits.ContainsKey(steamID))
            {
                PlayerKits.Add(steamID, new Dictionary<string, int[]>());
            }
            if (itemsID.Length > 0)
            {
                PlayerKits[steamID].Add(kitname, itemsID);
            }
            else
            {
                PlayerKits[steamID].Remove(kitname);
            }

            save();

        }

    }
}
