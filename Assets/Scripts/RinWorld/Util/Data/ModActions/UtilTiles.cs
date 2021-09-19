using System.IO;
using RinWorld.Util.IO;
using RinWorld.Util.Unity;

namespace RinWorld.Util.Data.ModActions
{
    public class UtilTiles : ModAction
    {

        private static readonly string UtilTilesFolder = Path.Combine(DataHolder.Loader.UtilFolder, "tiles");
        public string Name => GetType().Name;
        public int Priority => 5;
        public void Process(DataHolder.Loader loader, Mod mod)
        {
            var folder = Path.Combine(DataHolder.Loader.ModsFolder, mod.Name, UtilTilesFolder);
            foreach (var (name, tile) in Files.ReadTiles(folder, DataHolder.Loader.WorldCellSize))
                loader.AddUtilTile(name, new ImmutableTile(tile));
        }
    }
}