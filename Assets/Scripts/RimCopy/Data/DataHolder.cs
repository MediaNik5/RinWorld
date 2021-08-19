using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RimCopy.Colony;
using RimCopy.Exception;
using RimCopy.IO;
using RimCopy.World;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RimCopy.Data
{
    public static class DataHolder
    {
        private static readonly Dictionary<string, JObject> info = new Dictionary<string, JObject>();
        private static readonly Dictionary<(int, int), ColonyInfo> colonies = new Dictionary<(int, int), ColonyInfo>();

        private static readonly Dictionary<string, BiomePreset> biomes = new Dictionary<string, BiomePreset>();
        private static readonly Dictionary<string, ReliefPreset> reliefs = new Dictionary<string, ReliefPreset>();

        private static readonly Dictionary<string, Tile> utilTiles = new Dictionary<string, Tile>();

        private static readonly Dictionary<string, KeyCode> keySet = new Dictionary<string, KeyCode>();

        public static JObject GetInfo(string str)
        {
            return info[str];
        }

        public static ICollection<BiomePreset> GetAllBiomes()
        {
            return biomes.Values;
        }

        public static ICollection<ReliefPreset> GetAllReliefs()
        {
            return reliefs.Values;
        }

        public static ColonyInfo GetColony(int x, int y)
        {
            return colonies.ContainsKey((x, y)) ? colonies[(x, y)] : null;
        }

        public static Tile GetUtilTile(string name)
        {
            return utilTiles[name];
        }

        public static KeyCode GetKeyCode(string action)
        {
            return keySet[action];
        }

        internal static class ConstantLoader
        {
            private const string ModsFolder = "/mods/";
            private const string ModsPriority = ModsFolder + "priority.json";
            private const string UtilFolder = "/util/";
            private const string UtilTilesFolder = UtilFolder + "tiles/";
            private const string Keys = UtilFolder + "keys.json";
            private const string BiomesFolder = "/biomes/";
            private const string ReliefsFolder = "/reliefs/";
            private const string Biomes = "biomes.json";
            private const string Reliefs = "reliefs.json";
            
            public const int CellSize = 2048;

            private static string[] mods;

            public static void Load()
            {
                if (Game.GameState != GameState.LoadingConstants)
                    throw new InvalidStateException($"The game is not in {nameof(GameState.LoadingConstants)} state, " +
                                                    $"but asked to be {nameof(GameState.LoadingConstants)}");

                LoadModsPriority();

                LoadKeystrokes();
                LoadUtilTiles();
                LoadBiomes();
                LoadReliefs();
            }

            private static void LoadKeystrokes()
            {
                foreach (var mod in mods)
                    LoadKeystrokes(mod);
            }

            private static void LoadKeystrokes(string mod)
            {
                var path = ModsFolder + mod + Keys;
                var json = Files.ReadContentsGameFolder(path);
                var jObject = JObject.Parse(json);

                foreach (var jProperty in jObject)
                {
                    var parsed = Enum.TryParse(jProperty.Value.Value<string>(), out KeyCode keyCode);
                    if (!parsed)
                        throw new System.Exception($"Could not load key {jProperty.Key} for {jProperty.Value}");
                    keySet.Add(jProperty.Key, keyCode);
                }
            }

            private static void LoadUtilTiles()
            {
                foreach (var mod in mods)
                    LoadUtilTiles(mod);
            }

            private static void LoadUtilTiles(string mod)
            {
                var folder = ModsFolder + mod + UtilTilesFolder;
                foreach (var (name, tile) in Files.ReadTiles(folder, CellSize))
                    utilTiles.Add(name, tile);
            }

            private static void LoadModsPriority()
            {
                var priority = Files.ReadContentsGameFolder(ModsPriority);
                if (string.IsNullOrEmpty(priority))
                {
                    mods = new[] {"core"};
                    return;
                }

                var jArray = JArray.Parse(priority);
                mods = new string[jArray.Count];
                for (var i = 0; i < mods.Length; i++)
                    mods[i] = jArray[i].Value<string>();
            }

            private static void LoadBiomes()
            {
                foreach (var mod in mods)
                    LoadBiomes(mod);
            }

            private static void LoadBiomes(string mod)
            {
                var folder = ModsFolder + mod + BiomesFolder;
                var json = Files.ReadContentsGameFolder(folder + Biomes);
                var jArray = JArray.Parse(json);

                foreach (JObject biome in jArray)
                    LoadBiome(folder, biome);
            }

            private static void LoadBiome(string biomesFolder, JObject jBiome)
            {
                var name = jBiome["name"].Value<string>();
                var tile = Files.ReadTile(biomesFolder, CellSize, name);

                var biome = new BiomePreset(
                    tile,
                    name,
                    jBiome["min_height"].Value<float>(),
                    jBiome["min_moisture"].Value<float>(),
                    jBiome["min_heat"].Value<float>()
                );

                biomes.Add(name, biome);
            }

            private static void LoadReliefs()
            {
                foreach (var mod in mods)
                    LoadReliefs(mod);
            }

            private static void LoadReliefs(string mod)
            {
                var folder = ModsFolder + mod + ReliefsFolder;
                var json = Files.ReadContentsGameFolder(folder + Reliefs);
                var jArray = JArray.Parse(json);

                foreach (JObject biome in jArray)
                    LoadRelief(folder, biome);
            }

            private static void LoadRelief(string biomesFolder, JObject jRelief)
            {
                var name = jRelief["name"].Value<string>();
                var tile = Files.ReadTile(biomesFolder, CellSize, name);

                var relief = new ReliefPreset(
                    tile,
                    name,
                    jRelief["min_height"].Value<float>()
                );

                reliefs.Add(name, relief);
            }
        }
    }
}