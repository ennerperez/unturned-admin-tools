using CommandHandler;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Unturned
{
    internal class Specials : Module
    {

        #region TOP: global variables are initialized here

        internal static bool IsWarOn = false;

        #endregion

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), War, "war"));
            return _return;
        }
        internal override String GetHelp()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Strings.Get("HLP", "WarHelp"));

            return sb.ToString();
        }

        #region Commands

        internal static void War(CommandArgs args)
        {
            if (IsWarOn)
            {
                IsWarOn = !IsWarOn;
                NetworkChat.sendAlert(Strings.Get("MOD", "WarOver"));
                return;
            }

            IsWarOn = !IsWarOn;
            NetworkChat.sendAlert(Strings.Get("MOD", "WarComing"));

            try
            {

                string warmode = "ninja";
                if (args.Parameters.Count > 0)
                {
                    warmode = args.ParametersAsString.Trim().ToLower();
                }

                Items.Reset(args);
                Vehicles.Respawn(args);
                Zombies.Kill(args);
                //DestroyStructures(args);

                for (int i = 0; i < 5; i++)
                {
                    NetworkChat.sendAlert(String.Format(Strings.Get("MOD", "WarCount"), 5 - i));
                    System.Threading.Thread.Sleep(1000);
                }

                foreach (BetterNetworkUser item in UserList.users)
                {
                    Clothes cloth = item.player.gameObject.GetComponent<Clothes>();

                    int _backpack;
                    int _hat;
                    int _pants;
                    int _shirt;
                    int _vest;
                    int _primary;
                    int _secondary;
                    int _ammo;

                    switch (warmode)
                    {
                        case "green":
                            // Green
                            _backpack = 2004;
                            _hat = 11;
                            _pants = 5017;
                            _shirt = 4017;
                            _vest = 3002;
                            _primary = 7014;
                            _ammo = 25001;
                            _secondary = 8016;
                            break;
                        default:
                            // Ninja
                            _backpack = 2005;
                            _hat = 12;
                            _pants = 5018;
                            _shirt = 4018;
                            _vest = 3003;
                            _primary = 7007;
                            _ammo = 25001;
                            _secondary = 8015;
                            break;
                    }
                                        

                    Inventory inventory = item.player.gameObject.GetComponent<Inventory>();
                    inventory.drop();

                    cloth.changeBackpack(_backpack);
                    cloth.tellHat(_hat);
                    cloth.changePants(_pants);
                    cloth.changeShirt(_shirt);
                    cloth.changeVest(_vest);
                    cloth.saveAllClothing();
                    cloth.loadAllClothing();
                    
                    inventory.tryAddItem(_primary, 1);
                    inventory.tryAddItem(_ammo, 30);
                    inventory.tryAddItem(_secondary, 1);
                    inventory.tryAddItem(14022, 2);
                    inventory.tryAddItem(13000, 2);
                    inventory.tryAddItem(24000, 1);

                }

                NetworkChat.sendAlert(Strings.Get("MOD", "WarStart"));

            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
            }          

        }

        #endregion
        
    }
}
