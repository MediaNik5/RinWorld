using UnityEngine.Tilemaps;

namespace RinWorld.Buildings
{
    public class Building : Unit
    {
        public const int Layer = 0;

        private readonly string _name;
        public readonly Tile Tile;
        public readonly bool CanGoThrough;

        public Building(string name, Tile tile, bool canGoThrough, float goThroughRate)
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