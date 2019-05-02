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
        private static readonly string path = "Assets/Weapons";

        public static IList<Weapon> Weapons { get; private set; }

        private static FileSystemWatcher watcher;

        static WeaponStore()
        {
            //TODO: re-import whenever a file changes
            
            ImportWeapons();

            watcher = new FileSystemWatcher();
            
            watcher.Path = path;

            // Watch for changes in LastAccess and LastWrite times, and
            // the renaming of files or directories.
            watcher.NotifyFilter = NotifyFilters.LastWrite
                                   | NotifyFilters.FileName
                                   | NotifyFilters.LastAccess;

            // Only watch text files.
            watcher.Filter = "*.json";

            // Add event handlers.
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnRenamed;

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Debug.Log("File changed: " + e.FullPath);

            ImportWeapons();
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Debug.Log("File renamed: " + e.FullPath);

            ImportWeapons();
        }

        private static void ImportWeapons()
        {
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

                        //populate weapon for emitter so it can find projectiles
                        weapon.ProjectileEmitter.Weapon = weapon;

                        //populate weapon for all nested emitters
                        foreach (var projectile in weapon.Projectiles.Values)
                        {
                            if (projectile.Events != null)
                            {
                                foreach (var e in projectile.Events)
                                {
                                    foreach (var action in e.Actions)
                                    {
                                        if (action.Type == ProjectileAction.ActionTypes.Emit)
                                        {
                                            action.ProjectileEmitter.Weapon = weapon;
                                        }
                                    }
                                }
                            }
                        }

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
