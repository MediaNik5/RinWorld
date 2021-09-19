using System.Diagnostics.CodeAnalysis;
using System.IO;
using Newtonsoft.Json.Linq;
using RinWorld.Buildings;
using RinWorld.Util.Exception;
using RinWorld.Util.IO;
using RinWorld.World;

namespace RinWorld.Util.Data.ModActions
{
    public class Biomes : ModAction
    {

        private const string BiomesFolder = "biomes";
        private const string BiomesFile = "biomes.json";
        public string Name => GetType().Name;
        public int Priority => 6;
        public void Process(DataHolder.Loader loader, Mod mod)
        {
            string folder = Path.Combine(DataHolder.Loader.ModsFolder, mod.Name, BiomesFolder);
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
                mapCellPresets = LoadMapCellPresets(jBiome["points"] as JArray);
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
        private static MapCellPreset[] LoadMapCellPresets(JArray jPoints)
        {
            var points = new MapCellPreset[jPoints.Count];
            for (int i = 0; i < points.Length; i++)
            {
                var jPoint = jPoints[i] as JObject;
                Building building = null;
                Floor floor = null;
                if (jPoint.ContainsKey("building"))
                    building = (Building) DataHolder.GetUnit(jPoint["building"].Value<string>());
                if (jPoint.ContainsKey("floor"))
                    floor = (Floor) DataHolder.GetUnit(jPoint["floor"].Value<string>());

                if (floor == null && building == null)
                    throw new InvalidCellException($"CellPreset {jPoint["name"]} must have either \"building\" or \"floor\" property.");

                points[i] = new MapCellPreset(
                    building,
                    floor,
                    jPoint["name"].Value<string>(),
                    jPoint["min_height"].Value<float>(),
                    jPoint["min_worth"].Value<float>(),
                    jPoint["min_presence"].Value<float>()
                );
            }

            return points;
        }

    }
}