using System.IO;
using RinWorld.Util.IO;
using RinWorld.Util.Unity;

namespace RinWorld.Util.Data.ModActions
{
    public class UtilTiles : ModAction
    {

        private static readonly string UtilTilesFolder = Path.Combine(DataHolder.Loader.UtilFolder, "tiles");
        public override int Priority => 5;
        public override void Process(DataHolder.Loader loader, Mod mod, string modPath)
        {
            var folder = Path.Combine(modPath, UtilTilesFolder);
            foreach (var (name, tile) in Files.ReadTiles(folder, DataHolder.Loader.WorldCellSize))
                loader.AddUtilTile(name, new ImmutableTile(tile));
        }
    }
}