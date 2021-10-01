using Newtonsoft.Json.Linq;
using RinWorld.Util.Data;
using RinWorld.Util.Unity;

namespace RinWorld.Buildings
{
    public class FloorPreset : BuildingPreset
    {
        public new const int Layer = 1;

        protected FloorPreset(string name, ImmutableTile tile, bool canGoThrough, float goThroughRate) 
        : base(name, tile, canGoThrough, goThroughRate)
        {
        }

        private new static UnitPreset Of(JObject jObject)
        {
            return new FloorPreset(
                jObject["name"].Value<string>(),
                ImmutableTile.Of(jObject["tile"]),
                true,
                jObject["goThroughRate"].Value<float>()
            );
        }

        public override string ToString()
        {
            return "Floor " + base.ToString();
        }
    }
}