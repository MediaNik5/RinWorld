using RinWorld.Util.Unity;

namespace RinWorld.Things
{
    public class ThingPreset : UnitPreset
    {
        public const int Layer = 2;

        public readonly ImmutableTile Tile;
        protected ThingPreset(string name) : base(name)
        {
        }
    }
}