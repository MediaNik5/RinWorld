using RinWorld.Util.Unity;

namespace RinWorld.Things
{
    public class Thing : Unit
    {
        public const int Layer = 2;

        private string _name;
        public readonly ImmutableTile tile;
    }
}