using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Newtonsoft.Json.Linq;
using RinWorld.Buildings;
using RinWorld.Util.Exception;
using RinWorld.Util.IO;
using RinWorld.World;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RinWorld.Util.Data
{
    public static class DataHolder
    {
        private static readonly Dictionary<string, string> translation = new Dictionary<string, string>();

        private static readonly Dictionary<string, BiomePreset> biomes = new Dictionary<string, BiomePreset>();
        private static readonly Dictionary<string, ReliefPreset> reliefs = new Dictionary<string, ReliefPreset>();

        private static readonly Dictionary<string, Tile> tiles = new Dictionary<string, Tile>();
        private static readonly Dictionary<string, Tile> utilTiles = new Dictionary<string, Tile>();

        private static readonly Dictionary<string, KeyCode> keySet = new Dictionary<string, KeyCode>();

        private static readonly Dictionary<string, float> conditions = new Dictionary<string, float>();

        private static readonly Dictionary<string, Unit> units = new Dictionary<string, Unit>();
        private static readonly Tile defaultTile = ScriptableObject.CreateInstance<Tile>();

        static DataHolder()
        {
            var texture2D = new Texture2D(ConstantLoader.TileSize, ConstantLoader.TileSize);
            for (var x = 0; x < ConstantLoader.TileSize; x++)
            for (var y = 0; y < ConstantLoader.TileSize; y++)
            {
                var color = x / 64 + y / 64;
                texture2D.SetPixel(x, y, color % 2 == 1 ? Color.black : Color.yellow);
            }

            texture2D.Apply();
            defaultTile.sprite = Sprite.Create(
                texture2D,
                new Rect(0, 0, ConstantLoader.TileSize, ConstantLoader.TileSize),
                new Vector2(0.5f, 0.5f),
                100f,
                0U,
                SpriteMeshType.FullRect
            );
        }

        public static string Language { get; private set; }

        public static Unit GetUnit(string name)
        {
            Debug.Log(name);
            return units[name];
        }

        public static Tile GetTile(string name)
        {
            if (tiles.ContainsKey(name))
                return tiles[name];
            Debug.LogError($"Tile {name} was not found.");
            return defaultTile;
        }

        public static ICollection<BiomePreset> GetAllBiomes()
        {
            return biomes.Values;
        }

        public static ICollection<ReliefPreset> GetAllReliefs()
        {
            return reliefs.Values;
        }

        public static Tile GetUtilTile(string name)
        {
            return utilTiles[name];
        }

        public static KeyCode GetKeyCode(string action)
        {
            return keySet[action];
        }

        public static float GetCondition(string condition)
        {
            return conditions[condition];
        }

        public static void Load()
        {
            ConstantLoader.Load();
        }

        public static void SetLanguage(string language)
        {
            Files.WriteContentsGameFolder(ConstantLoader.Language, $"{{\n\t\"language\": \"{language}\"\n}}");
            ConstantLoader.LoadLanguage();
            ConstantLoader.LoadTranslations();
        }

        public static string Translate(string key)
        {
            return translation.ContainsKey(key) ? translation[key] : key;
        }

        private static class ConstantLoader
        {
            private const string ModsFolder = "/mods/";
            private const string ModsPriority = ModsFolder + "priority.json";
            private const string UtilFolder = "/util/";
            private const string UtilTilesFolder = UtilFolder + "tiles/";
            private const string TilesFolder = "/tiles/";
            private const string Units = "/units.json";
            private const string Conditions = "/conditions.json";
            private const string Keys = UtilFolder + "keys.json";
            public const string Language = "/language.json";
            private const string LanguagesFolder = "/languages/";
            private const string BiomesFolder = "/biomes/";
            private const string ReliefsFolder = "/reliefs/";
            private const string Biomes = "biomes.json";
            private const string Reliefs = "reliefs.json";

            public const int CellSize = 2048;
            public const int TileSize = 128;

            private static string[] _mods;

            
            public static void Load()
            {
                if (Game.GameState != GameState.LoadingConstants)
                    throw new InvalidStateException($"The game is not in {nameof(GameState.LoadingConstants)} state, " +
                                                    $"but asked to be {nameof(GameState.LoadingConstants)}");

                LoadModsPriority();

                LoadLanguage();
                LoadTranslations();

                LoadTiles();
                LoadUnits();
                
                LoadKeystrokes();
                LoadConditions();
                LoadUtilTiles();
                LoadBiomes();
                LoadReliefs();
            }

            private static void LoadTiles()
            {
                foreach (var mod in _mods)
                    LoadTiles(mod);
            }

            private static void LoadTiles(string mod)
            {
                var folder = ModsFolder + mod + TilesFolder;
                foreach (var (name, tile) in Files.ReadTiles(folder, TileSize))
                    tiles.Add(name, tile);
            }

            private static void LoadUnits()
            {
                foreach (var mod in _mods)
                    LoadUnits(mod);
            }

            private static void LoadUnits(string mod)
            {
                var json = Files.ReadContentsGameFolder(ModsFolder + mod + Units);
                var jUnits = JArray.Parse(json);
                var folder = ModsFolder + mod + TilesFolder;

                foreach (JObject jUnit in jUnits)
                    units.Add(jUnit["name"].Value<string>(), Unit.Of(jUnit));
            }

            public static void LoadLanguage()
            {
                var jsonLanguage = Files.ReadContentsGameFolder(Language);
                if (jsonLanguage == null)
                {
                    DataHolder.Language = CultureInfo.CurrentUICulture.Name;
                    Files.WriteContentsGameFolder(Language, $"{{\n\t\"language\": \"{DataHolder.Language}\"\n}}");
                }
                else
                {
                    DataHolder.Language = JObject.Parse(jsonLanguage)["language"].Value<string>();
                }

                CheckLanguageSupport();
            }

            private static void CheckLanguageSupport()
            {
                if (!Files.FileGameFolderExists(ModsFolder + "core" + LanguagesFolder + DataHolder.Language + ".json"))
                {
                    DataHolder.Language = "en-US";
                    Files.WriteContentsGameFolder(Language, "{\n\t\"language\": \"en-US\"\n}");
                }
            }

            public static void LoadTranslations()
            {
                foreach (var mod in _mods)
                    LoadTranslations(mod);
            }

            private static void LoadTranslations(string mod)
            {
                var languageSet =
                    Files.ReadContentsGameFolder(ModsFolder + mod + LanguagesFolder + DataHolder.Language + ".json");
                var jLanguageSet = JObject.Parse(languageSet);
                foreach (var jProperty in jLanguageSet)
                    translation.Add(jProperty.Key, jProperty.Value.Value<string>());
            }

            private static void LoadKeystrokes()
            {
                foreach (var mod in _mods)
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

            private static void LoadConditions()
            {
                foreach (var mod in _mods)
                    LoadConditions(mod);
            }

            private static void LoadConditions(string mod)
            {
                var path = ModsFolder + mod + Conditions;
                var json = Files.ReadContentsGameFolder(path);
                var jObject = JObject.Parse(json);

                foreach (var jCondition in jObject)
                    conditions.Add(jCondition.Key, jCondition.Value.Value<float>());
            }

            private static void LoadUtilTiles()
            {
                foreach (var mod in _mods)
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
                    _mods = new[] {"core"};
                    return;
                }

                var jArray = JArray.Parse(priority);
                _mods = new string[jArray.Count];
                for (var i = 0; i < _mods.Length; i++)
                    _mods[i] = jArray[i].Value<string>();
            }

            private static void LoadBiomes()
            {
                foreach (var mod in _mods)
                    LoadBiomes(mod);
            }

            private static void LoadBiomes(string mod)
            {
                var folder = ModsFolder + mod + BiomesFolder;
                var json = Files.ReadContentsGameFolder(folder + Biomes);
                var jArray = JArray.Parse(json);

                foreach (JObject biome in jArray)
                    LoadBiome(folder, biome);

                Files.WriteContentsGameFolder("/biomesExcluded1.txt", BiomePreset.CheckBiomeCoverage());
            }

            private static void LoadBiome(string biomesFolder, JObject jBiome)
            {
                var name = jBiome["name"].Value<string>();
                var tile = Files.ReadTile(biomesFolder, CellSize, name);

                var points = LoadPoints(jBiome["points"] as JArray);

                var biome = new BiomePreset(
                    tile,
                    name,
                    jBiome["min_height"].Value<float>(),
                    jBiome["min_moisture"].Value<float>(),
                    jBiome["min_heat"].Value<float>(),
                    jBiome["max_height"].Value<float>(),
                    jBiome["max_moisture"].Value<float>(),
                    jBiome["max_heat"].Value<float>(),
                    points
                );

                biomes.Add(name, biome);
            }

            [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
            private static PointPreset[] LoadPoints(JArray jPoints)
            {
                var points = new PointPreset[jPoints.Count];
                for (var i = 0; i < points.Length; i++)
                {
                    var jPoint = jPoints[i] as JObject;
                    Building building = null;
                    if (jPoint.ContainsKey("building"))
                        building = (Building) GetUnit(jPoint["building"].Value<string>());

                    points[i] = new PointPreset(
                        building,
                        (Floor) GetUnit(jPoint["floor"].Value<string>()),
                        jPoint["name"].Value<string>(),
                        jPoint["min_height"].Value<float>(),
                        jPoint["min_worth"].Value<float>(),
                        jPoint["min_presence"].Value<float>()
                    );
                }

                return points;
            }

            private static void LoadReliefs()
            {
                foreach (var mod in _mods)
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