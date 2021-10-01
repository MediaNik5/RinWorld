using System.Diagnostics.CodeAnalysis;
using System.IO;
using Newtonsoft.Json.Linq;
using RinWorld.Buildings;
using RinWorld.Util.Exception;
using RinWorld.Util.IO;
using RinWorld.Worlds;
using RinWorld.Worlds.Maps;

namespace RinWorld.Util.Data.ModActions
{
    public class Biomes : ModAction
    {

        private const string BiomesFolder = "biomes";
        private const string BiomesFile = "biomes.json";
        public override int Priority => 6;
        public override void Process(DataHolder.Loader loader, Mod mod, string modPath)
        {
            string folder = Path.Combine(modPath, BiomesFolder);
            string json = Files.ReadContentsGameFolder(Path.Combine(folder, BiomesFile));
            var jBiomes = JArray.Parse(json);

            foreach (JObject jBiome in jBiomes)
                LoadBiome(loader, folder, jBiome);
        }
        

        private static void LoadBiome(DataHolder.Loader loader, string biomesFolder, JObject jBiome)
        {
            var name = jBiome["name"].Value<string>();
            var tile = Files.ReadTile(biomesFolder, DataHolder.Loader.WorldCellSize, name);

            MapCellPreset[] mapCellPresets;
            
            try
            {
                mapCellPresets = LoadMapCellPresets(jBiome["cells"] as JArray);
            }
            catch (InvalidCellException e)
            {
                throw new InvalidFileException(Path.Combine(biomesFolder, BiomesFile), e);
            }

            var biome = new BiomePreset(
                tile,
                name,
                jBiome["min_height"].Value<float>(),
                jBiome["min_moisture"].Value<float>(),
                jBiome["min_heat"].Value<float>(),
                jBiome["max_height"].Value<float>(),
                jBiome["max_moisture"].Value<float>(),
                jBiome["max_heat"].Value<float>(),
                mapCellPresets
            );

            loader.AddBiome(name, biome);
        }

        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        private static MapCellPreset[] LoadMapCellPresets(JArray jCells)
        {
            var cellss = new MapCellPreset[jCells.Count];
            for (int i = 0; i < cellss.Length; i++)
            {
                var jCell = jCells[i] as JObject;
                BuildingPreset buildingPreset = null;
                FloorPreset floorPreset = null;
                if (jCell.ContainsKey("building"))
                    buildingPreset = (BuildingPreset) DataHolder.GetUnit(jCell["building"].Value<string>());
                if (jCell.ContainsKey("floor"))
                    floorPreset = (FloorPreset) DataHolder.GetUnit(jCell["floor"].Value<string>());

                if (floorPreset == null && buildingPreset == null)
                    throw new InvalidCellException($"CellPreset {jCell["name"]} must have either \"building\" or \"floor\" property.");

                cellss[i] = new MapCellPreset(
                    buildingPreset,
                    floorPreset,
                    jCell["name"].Value<string>(),
                    jCell["min_height"].Value<float>(),
                    jCell["min_worth"].Value<float>(),
                    jCell["min_presence"].Value<float>()
                );
            }

            return cellss;
        }

    }
}