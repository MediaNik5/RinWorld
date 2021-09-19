using System.IO;
using RinWorld.Util.IO;
using RinWorld.Util.Unity;

namespace RinWorld.Util.Data.ModActions
{
    public class Tiles : ModAction
    {

        private const string TilesFolder = "tiles";
        public string Name => GetType().Name;
        public int Priority => 1;

        
        public void Process(DataHolder.Loader loader, Mod mod)
        {
            var folder = Path.Combine(DataHolder.Loader.ModsFolder, mod.Name, TilesFolder);
            foreach (var (name, tile) in Files.ReadTiles(folder, DataHolder.Loader.MapCellSize))
                loader.AddTile(name, new ImmutableTile(tile));
        }
    }
}