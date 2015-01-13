using CommandHandler;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Unturned
{
    internal class Zombies : Module
    {

        #region TOP: global variables are initialized here



        #endregion

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Spawn, "zombie", "z"));
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Respawn, "respawnzombies", "zr"));
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Kill, "killzombies", "zk"));
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Land, "zombieland", "zl"));
            return _return;
        }
        internal override String GetHelp()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Strings.Get("HLP", "ZombieHelp"));
            sb.AppendLine(Strings.Get("HLP", "ZombiesRespawnHelp"));
            sb.AppendLine(Strings.Get("HLP", "ZombiesKillsHelp"));
            sb.AppendLine(Strings.Get("HLP", "ZombieLandHelp"));

            return sb.ToString();
        }

        #region Commands

        internal static void Spawn(CommandArgs args)
        {
            Vector3 location = args.sender.position;
            Quaternion rotation = args.sender.rotation;
            Vector3 newPos = new Vector3(location[0] + 1, location[1] + 1, location[2] - 1);

            spawn(newPos);

        }
        internal static void Respawn(CommandArgs args)
        {
            SpawnAnimals.reset();
            NetworkChat.sendAlert(String.Format(Strings.Get("MOD", "ZombiesRespawn"), args.sender.name));
        }
        internal static void Kill(CommandArgs args)
        {
            int kills = kill();
            NetworkChat.sendAlert(String.Format(Strings.Get("MOD", "ZombiesKills"), args.sender.name, kills));
        }
        internal static void Land(CommandArgs args)
        {

            int amount = 0;
            if (args.Parameters.Count > 0) { amount = int.Parse(args.Parameters[0]); }

            if (land(amount))
            {
                NetworkChat.sendAlert(string.Format(Strings.Get("MOD", "ZombieLand"), args.sender.name));
            }

        }

        #endregion

        private static void spawn(Vector3 pos)
        {
            try
            {
                Zombie[] mapZombies = UnityEngine.Object.FindObjectsOfType(typeof(Zombie)) as Zombie[];

                int random = UnityEngine.Random.Range(0, mapZombies.Length);
                Zombie randomZombie = mapZombies[random];

                randomZombie.transform.position = pos;
            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
            }

        }
        private static int kill()
        {
            try
            {
                Zombie[] mapZombies = UnityEngine.Object.FindObjectsOfType(typeof(Zombie)) as Zombie[];
                int counter = 0;
                foreach (Zombie Zombie in mapZombies)
                {
                    Zombie.damage(500);
                    counter++;
                }
                return counter;
            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
                return 0;
            }
        }
        private static bool land(int amount = 0)
        {
            try
            {

                SpawnAnimals.reset();

                Zombie[] mapZombies = UnityEngine.Object.FindObjectsOfType(typeof(Zombie)) as Zombie[];
                BetterNetworkUser[] mapUsers = UserList.users.ToArray();

                if (amount == 0) { amount = (int)Math.Round((decimal)mapZombies.Length / mapUsers.Length); }

                // Security margin
                if ((amount > 20) || (amount > mapZombies.Length)) { amount = 20; }

                foreach (BetterNetworkUser item in mapUsers)
                {

                    Vector3 location = item.position;
                    Quaternion rotation = item.rotation;

                    foreach (Zombie itemz in mapZombies)
                    {

                        int random = UnityEngine.Random.Range(10, 15);
                        Vector3 newPos = new Vector3(location[0] + random, location[1] + 5, location[2] - random);

                        itemz.transform.position = newPos;
                        amount--;
                        if (amount == 0) { break; }
                    }

                    amount = (int)Math.Round((decimal)mapZombies.Length / mapUsers.Length);
                    if ((amount > 20) || (amount > mapZombies.Length)) { amount = 20; }

                }

                return true;

            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
                return false;
            }
        }

    }
}
