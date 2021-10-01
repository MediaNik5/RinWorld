using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using RinWorld.Characteristics;
using RinWorld.Entities.Body;
using RinWorld.Util.Data.ModActions;
using RinWorld.Util.Exception;
using RinWorld.Util.IO;
using RinWorld.Util.Unity;
using RinWorld.Worlds;
using UnityEngine;

namespace RinWorld.Util.Data
{
    public static class DataHolder
    {
        private static readonly Dictionary<string, string> translations = new Dictionary<string, string>();

        private static readonly Dictionary<string, BiomePreset> biomes = new Dictionary<string, BiomePreset>();
        private static readonly Dictionary<string, ReliefPreset> reliefs = new Dictionary<string, ReliefPreset>();

        private static readonly Dictionary<string, ImmutableTile> tiles = new Dictionary<string, ImmutableTile>();
        private static readonly Dictionary<string, ImmutableTile> utilTiles = new Dictionary<string, ImmutableTile>();

        private static readonly Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();

        private static readonly Dictionary<string, float> conditions = new Dictionary<string, float>();

        private static readonly Dictionary<string, UnitPreset> units = new Dictionary<string, UnitPreset>();
        private static readonly ImmutableTile DefaultTile = new ImmutableTile(Utils.DefaultTile);

        public static string Language { get; private set; }
        
        public static List<T> GetUnits<T>() where T : UnitPreset
        {
            if (typeof(T) == typeof(UnitPreset))
                return (List<T>)(object)new List<UnitPreset>(units.Values);

            return units.Values.Where(unit => unit is T).Cast<T>().ToList();
        }
        
        public static UnitPreset GetUnit(string name)
        {
            if(units.ContainsKey(name))
                return units[name];
            throw new KeyNotFoundException($"Key {name} was not found in {nameof(units)}");
        }

        public static ImmutableTile GetTile(string name)
        {
            if (tiles.ContainsKey(name))
                return tiles[name];
            Debug.LogError($"Tile {name} was not found, using default tile instead.");
            return DefaultTile;
        }

        public static ICollection<BiomePreset> GetAllBiomes()
        {
            return biomes.Values;
        }

        public static ICollection<ReliefPreset> GetAllReliefs()
        {
            return reliefs.Values;
        }

        public static ImmutableTile GetUtilTile(string name)
        {
            if (utilTiles.ContainsKey(name))
                return utilTiles[name];
            Debug.LogError($"UtilTile {name} was not found, using default tile instead.");
            return DefaultTile;
        }

        public static KeyCode GetKeyCode(string action)
        {
            return keys[action];
        }

        public static float GetCondition(string condition)
        {
            return conditions[condition];
        }

        private static Loader loader;
        public static void Load()
        {
            Loader._init();
            loader.Load();
        }

        public static void SetLanguage(string language)
        {
            Files.WriteContentsGameFolder(LanguageHolder.LanguageFile, $"{{\n\t\"language\": \"{language}\"\n}}");
            LanguageHolder.LoadLanguage();
            loader.Load<Translations>();
        }

        public static string Translate(string key)
        {
            return translations.ContainsKey(key) ? translations[key] : key;
        }
        
        private static class LanguageHolder
        {
            public const string LanguageFile = "language.json";
            public static void LoadLanguage()
            {
                var jsonLanguage = Files.ReadContentsGameFolder(LanguageFile);
                if (jsonLanguage == null)
                {
                    Language = CultureInfo.CurrentUICulture.Name;
                    Files.WriteContentsGameFolder(LanguageFile, $"{{\n\t\"language\": \"{Language}\"\n}}");
                }
                else
                {
                    Language = JObject.Parse(jsonLanguage)["language"].Value<string>();
                }

                CheckLanguageSupport();
            }

            private static void CheckLanguageSupport()
            {
                string path = Path.Combine(Loader.ModsFolder, "core", Loader.LanguagesFolder, Language + ".json");
                if (!Files.FileGameFolderExists(path))
                {
                    Language = "en-US";
                    Files.WriteContentsGameFolder(LanguageFile, "{\n\t\"language\": \"en-US\"\n}");
                }
            }
        }

        public class Loader
        {            
            public const string ModsFolder = "mods";
            private const string ModsPriority = ModsFolder + "priority.json";
            public const string LanguagesFolder = "languages";
            public const string UtilFolder = "util";

            public const int WorldCellSize = 2048;
            public const int MapCellSize = 128;

            private Mod[] _mods;
            internal static void _init() {}
            static Loader()
            {
                loader = new Loader();
            }
            protected Loader() {}

            public void Load()
            {
                if (Game.GameState != GameState.LoadingConstants)
                    throw new InvalidStateException($"The game is not in {nameof(GameState.LoadingConstants)} state, " +
                                                    $"but asked to be {nameof(GameState.LoadingConstants)}");

                LanguageHolder.LoadLanguage();
                LoadMods();
                
                Test.Test.BiomeCoverage();
            }
            public void Load<TAction>() where TAction : ModAction=> 
                Mods.ProcessAction<TAction>(this, _mods);
            
            private void LoadMods()
            {
                LoadListOfModsFromDisk();
                LoadActionsWithPriority();
            }

            private void LoadListOfModsFromDisk()
            {
                string stringPriorities = Files.ReadContentsGameFolder(ModsPriority);
                if (string.IsNullOrEmpty(stringPriorities))
                {
                    _mods = new[] { new Mod("core", 0) };
                    Files.WriteContentsGameFolder(ModsPriority, "[\n\t\"core\"\n]");
                    return;
                }

                var jMods = JArray.Parse(stringPriorities);
                _mods = new Mod[jMods.Count];
                for (int i = 0; i < _mods.Length; i++)
                    _mods[i] = new Mod(jMods[i].Value<string>(), i);
            }
            private void LoadActionsWithPriority() => 
                Mods.Process(this, _mods);

#region Adders

            public void ResetTranslation() => 
                translations.Clear();
            public void AddTranslation(string name, string value) => 
                translations.Add(name, value);
            public void AddTile(string name, ImmutableTile value) => 
                tiles.Add(name, value);
            public void AddUnit(string name, UnitPreset value) => 
                units.Add(name, value);
            public void AddKey(string name, KeyCode value) => 
                keys.Add(name, value);
            public void AddCondition(string name, float value) => 
                conditions.Add(name, value);
            public void AddUtilTile(string name, ImmutableTile immutableTile) => 
                utilTiles.Add(name, immutableTile);
            public void AddBiome(string name, BiomePreset value) => 
                biomes.Add(name, value);
            public void AddRelief(string name, ReliefPreset value) => 
                reliefs.Add(name, value);
#endregion

        }
    }
}