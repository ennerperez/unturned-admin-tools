using CommandHandler;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unturned
{
    internal class Animals : Module
    {

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Spawn, "animal", "a"));
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Respawn, "respawnanimals", "ar"));
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Kill, "killanimal", "ak"));
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Wild, "wild", "aw")); // Use /wild <#>
            return _return;
        }
        internal override String GetHelp()
        {
            return null;
        }

        #region Commands

        internal static void Kill(CommandArgs args)
        {
            SpawnAnimals.reset();
            Animal[] Animals = UnityEngine.Object.FindObjectsOfType(typeof(Animal)) as Animal[];
            foreach (Animal animal in Animals)
            {
                animal.die();
            }
            NetworkChat.sendAlert(String.Format("{0} has killed {1} animals.", args.sender.name, Animals.Length));
        }

        internal static void Respawn(CommandArgs args)
        {
            SpawnAnimals.reset();
            Animal[] Animals = UnityEngine.Object.FindObjectsOfType(typeof(Animal)) as Animal[];
            NetworkChat.sendAlert(String.Format("{0} has re-spawned {1} animals.", args.sender.name, Animals.Length));
        }
        internal static void Spawn(CommandArgs args)
        {
            Vector3 location = args.sender.position;
            Quaternion rotation = args.sender.rotation;
            Vector3 newPos = new Vector3(location[0] + 1, location[1] + 1, location[2] - 1);

            Animal[] mapAnimals = UnityEngine.Object.FindObjectsOfType(typeof(Animal)) as Animal[];

            int random = UnityEngine.Random.Range(0, mapAnimals.Length);
            Animal randomAnimal = mapAnimals[random];

            randomAnimal.transform.position = newPos;

        }

        internal static void Wild(CommandArgs args)
        {
            Vector3 location = args.sender.position;
            Quaternion rotation = args.sender.rotation;
            Vector3 newPos = new Vector3(location[0] + 5, location[1] + 5, location[2] - 5);

            SpawnAnimals.reset();
            int amount = 0;
            if (args.Parameters.Count > 0) { amount = int.Parse(args.Parameters[0]); }
            Animal[] mapAnimals = UnityEngine.Object.FindObjectsOfType(typeof(Animal)) as Animal[];
            if (amount > mapAnimals.Length) { amount = mapAnimals.Length; }
            foreach (Animal item in mapAnimals)
            {
                item.transform.position = newPos;
                amount--;
                if ((args.Parameters.Count != 0) & (amount == 0)) { break; }
            }
            NetworkChat.sendAlert(string.Format("{0} opened the zoo's gate.", args.sender.name));

        }

        #endregion

    }

}
