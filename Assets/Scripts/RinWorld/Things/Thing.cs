using UnityEngine.Tilemaps;

namespace RinWorld.Things
{
    public class Thing : Unit
    {
        public const int Layer = 2;

        private string _name;
        public readonly Tile tile;
    }
}