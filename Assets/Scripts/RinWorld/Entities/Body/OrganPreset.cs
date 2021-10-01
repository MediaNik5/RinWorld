using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using RinWorld.Util;
using RinWorld.Util.Data;
using RinWorld.Util.Unity;

namespace RinWorld.Entities.Body
{
    public class OrganPreset : BodyPartPreset
    {

        public OrganPreset(
            string name,
            float maxDurability,
            int normalNumber,
            int maxAvailableNumber,
            string jointedTo,
            [CanBeNull] ImmutableTile tile,
            Dictionary<string, float> influences
        ) : base(name, maxDurability, normalNumber, maxAvailableNumber, jointedTo, tile, influences)
        {
        }
        // ReSharper disable once UnusedMember.Local
        private new static UnitPreset Of(JObject jOrgan)
        {
            ImmutableTile tile = null;
            if (jOrgan.ContainsKey("tile"))
                tile = ImmutableTile.Of(jOrgan["tile"]);

            return new LimbPreset(
                jOrgan["name"].Value<string>(),
                jOrgan["maxDurability"].Value<float>(),
                jOrgan["normalNumber"].Value<int>(),
                jOrgan["maxAvailableNumber"].Value<int>(),
                tile,
                Utils.extractDictionary<string, float>(jOrgan["influences"] as JObject, s => s)
            );
        }
    }
}