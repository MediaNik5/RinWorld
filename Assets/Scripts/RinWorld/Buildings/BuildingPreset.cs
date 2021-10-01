using System;
using JetBrains.Annotations;
using RinWorld.Util.Unity;

namespace RinWorld.Buildings
{
    public class BuildingPreset : UnitPreset
    {
        public const int Layer = 0;

        public readonly ImmutableTile Tile;
        public readonly bool CanGoThrough;
        public float GoThroughRate { get; }

        
        protected BuildingPreset(
            string name, 
            ImmutableTile tile, 
            bool canGoThrough, 
            float goThroughRate
        ) : base(name)
        {
            Tile = tile;
            CanGoThrough = canGoThrough;
            GoThroughRate = goThroughRate;
        }

        public override string ToString()
        {
            return "Building " + base.ToString();
        }
    }
}