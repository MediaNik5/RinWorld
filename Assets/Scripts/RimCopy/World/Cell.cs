using RimCopy.Attribute;
using RimCopy.Colony;
using RimCopy.Data;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RimCopy.World
{
    public class Cell : Unit, Renderable
    {
        [DontSave] public readonly int x;
        [DontSave] public readonly int y;
        [DontSave] private readonly float _height;
        [DontSave] private readonly float _moisture;
        [DontSave] private readonly float _heat;
        [DontSave] private readonly BiomePreset _biome;
        [DontSave] private readonly ReliefPreset _relief;
        [DontSave] private ColonyInfo _colony;

        public bool Changed { get; } = false;

        private Cell(int x, int y, float height, float moisture, float heat)
        {
            this.x = x;
            this.y = y;
            _height = height;
            _moisture = moisture;
            _heat = heat;
            _biome = BiomePreset.For(_height, _moisture, _heat);
            _relief = ReliefPreset.For(_height);
            _colony = DataHolder.GetColony(x, y);
        }

        public static Cell Of(int x, int y, float height, float moisture, float heat)
        {
            return new Cell(x, y, height, moisture, heat);
        }


        public void StartRender(Tilemap[] tilemaps)
        {
            tilemaps[0].SetTile(new Vector3Int(x, y, 0), _biome.Tile);
            tilemaps[1].SetTile(new Vector3Int(x, y, 0), _relief.Tile);
        }

        public void Render(Tilemap[] tilemaps)
        {
        }
    }
}