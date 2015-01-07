using CommandHandler;
using System;
using UnityEngine;

namespace Unturned
{
    public static class Zombies
    {
        #region TOP: global variables are initialized here

        #endregion

        internal static void GetCommands()
        {
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Spawn, "zombie", "z"));
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Respawn, "respawnzombies", "zr"));
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Kill, "killzombies", "zk"));
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Apocalypse, "apocalypse", "za"));
        }

        internal static void Spawn(CommandArgs args)
        {
            Vector3 location = args.sender.position;
            Quaternion rotation = args.sender.rotation;
            Vector3 newPos = new Vector3(location[0] + 1, location[1] + 1, location[2] - 1);

            Zombie[] mapZombies = UnityEngine.Object.FindObjectsOfType(typeof(Zombie)) as Zombie[];

            int random = UnityEngine.Random.Range(0, mapZombies.Length);
            Zombie randomZombie = mapZombies[random];

            randomZombie.transform.position = newPos;

        }
        internal static void Respawn(CommandArgs args)
        {
            SpawnAnimals.reset();
            NetworkChat.sendAlert(String.Format("{0} has re-spawned all zombies.", args.sender.name));
        }

        internal static void Kill(CommandArgs args)
        {
            Zombie[] Zombies = UnityEngine.Object.FindObjectsOfType(typeof(Zombie)) as Zombie[];
            foreach (Zombie Zombie in Zombies)
            {
                Zombie.damage(500);
            }
            NetworkChat.sendAlert(String.Format("{0} has killed {1} zombies.", args.sender.name, Zombies.Length));
        }

        internal static void Apocalypse(CommandArgs args)
        {

            SpawnAnimals.reset();
            Zombie[] mapZombies = UnityEngine.Object.FindObjectsOfType(typeof(Zombie)) as Zombie[];
            BetterNetworkUser[] mapUsers = UserList.users.ToArray();

            int amount = (int)Math.Round((decimal)mapZombies.Length / mapUsers.Length);
            if (args.Parameters.Count > 0) { amount = int.Parse(args.Parameters[0]); }

            // Security margen
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
            NetworkChat.sendAlert(string.Format("{0} opened the hell's gates.", args.sender.name));

        }


    }
}
