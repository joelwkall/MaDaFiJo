using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets
{
    [InitializeOnLoad]
    class WeaponStore
    {
        public static IList<Weapon> Weapons { get; private set; }

        static WeaponStore()
        {
            string path = "Assets/Weapons";
            var files = Directory.GetFiles(path)
                .Where(p => Path.GetExtension(p).Equals(".json", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            var weapons = new List<Weapon>(files.Length);

            foreach (var file in files)
            {
                using (var reader = new StreamReader(file))
                {
                    try
                    {
                        var text = reader.ReadToEnd();

                        var weapon = JsonConvert.DeserializeObject<Weapon>(text);
                        var name = Path.GetFileNameWithoutExtension(file);

                        weapon.Name = name;
                        weapon.ProjectileEmitter.Weapon = weapon;

                        //TODO: validate all data

                        weapons.Add(weapon);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error when importing weapon file " + file + ": " + ex.Message + ".", ex);
                    }
                }
            }

            Weapons = weapons;
        }
    }
}
