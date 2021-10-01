using System.IO;
using Newtonsoft.Json.Linq;
using RinWorld.Util.IO;
using RinWorld.Util.Unity;
using RinWorld.Worlds;

namespace RinWorld.Util.Data.ModActions
{
    public class Relief : ModAction
    {
        private const string ReliefsFolder = "reliefs";
        private const string Reliefs = "reliefs.json";
        public override int Priority => 7;
        public override void Process(DataHolder.Loader loader, Mod mod, string modPath)
        {
            string folder = Path.Combine(modPath, ReliefsFolder);
            string json = Files.ReadContentsGameFolder(Path.Combine(folder, Reliefs));
            var jArray = JArray.Parse(json);

            foreach (JObject biome in jArray)
                LoadRelief(loader, folder, biome);
        }

        private static void LoadRelief(DataHolder.Loader loader, string biomesFolder, JObject jRelief)
        {
            var name = jRelief["name"].Value<string>();
            var tile = Files.ReadTile(biomesFolder, DataHolder.Loader.WorldCellSize, name);

            var relief = new ReliefPreset(
                new ImmutableTile(tile),
                name,
                jRelief["min_height"].Value<float>()
            );

            loader.AddRelief(name, relief);
        }
    }
}