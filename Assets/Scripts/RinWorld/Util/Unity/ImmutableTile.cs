using UnityEngine;
using UnityEngine.Tilemaps;

namespace RinWorld.Util.Unity
{
    public class ImmutableTile
    {
        private readonly Tile _tile;

        public ImmutableTile(Tile tile)
        {
            _tile = tile;
        }

        public void ApplyFor(Tilemap tilemap, Vector3Int position)
        {
            tilemap.SetTile(position, _tile);
        }

        public void ApplyFor(Tilemap tilemap, int x, int y)
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), _tile);
        }
    }
}