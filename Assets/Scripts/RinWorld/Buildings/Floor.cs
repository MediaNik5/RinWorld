using Newtonsoft.Json.Linq;
using RinWorld.Util.Data;
using UnityEngine.Tilemaps;

namespace RinWorld.Buildings
{
    public class Floor : Building
    {
        public const int Layer = 1;

        public Floor(string name, Tile tile, bool canGoThrough, float goThroughRate) : base(name, tile, canGoThrough,
            goThroughRate)
        {
        }

        private static Unit Of(JObject jObject)
        {
            return new Floor(
                jObject["name"].Value<string>(),
                DataHolder.GetTile(jObject["tile"].Value<string>()),
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