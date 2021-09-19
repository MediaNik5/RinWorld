using RinWorld.Util.Unity;

namespace RinWorld.Buildings
{
    public class Building : Unit
    {
        public const int Layer = 0;

        private readonly string _name;
        public readonly ImmutableTile Tile;
        public readonly bool CanGoThrough;

        public Building(string name, ImmutableTile tile, bool canGoThrough, float goThroughRate)
        {
            _name = name;
            Tile = tile;
            CanGoThrough = canGoThrough;
            GoThroughRate = goThroughRate;
        }

        public float GoThroughRate { get; }

        public override string ToString()
        {
            return "Building " + _name + ", " + base.ToString();
        }
    }
}