using System.IO;
using RinWorld.Util.IO;
using RinWorld.Util.Unity;

namespace RinWorld.Util.Data.ModActions
{
    public class Tiles : ModAction
    {

        private const string TilesFolder = "tiles";
        public override int Priority => 1;

        
        public override void Process(DataHolder.Loader loader, Mod mod, string modPath)
        {
            var folder = Path.Combine(modPath, TilesFolder);
            foreach (var (name, tile) in Files.ReadTiles(folder, 2))
                loader.AddTile(name, new ImmutableTile(tile));
        }
    }
}