﻿using Newtonsoft.Json;
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
        public static IDictionary<string,Weapon> Weapons { get; private set; }

        static WeaponStore()
        {
            string path = "Assets/Weapons";
            var files = Directory.GetFiles(path)
                .Where(p => Path.GetExtension(p).Equals(".json", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            var weapons = new Dictionary<string, Weapon>(files.Length);

            foreach (var file in files)
            {
                using (var reader = new StreamReader(file))
                {
                    var text = reader.ReadToEnd();

                    var weapon = JsonConvert.DeserializeObject<Weapon>(text);
                    var name = Path.GetFileNameWithoutExtension(file);

                    weapons.Add(name, weapon);
                }
            }

            Weapons = weapons;
        }
    }
}