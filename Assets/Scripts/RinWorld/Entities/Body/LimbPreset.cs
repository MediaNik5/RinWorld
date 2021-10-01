using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using RinWorld.Util;
using RinWorld.Util.Data;
using RinWorld.Util.Unity;

namespace RinWorld.Entities.Body
{
    public class LimbPreset : BodyPartPreset
    {
        public LimbPreset(
            string name, 
            float maxDurability, 
            int normalNumber, 
            int maxAvailableNumber,
            [CanBeNull] ImmutableTile tile, 
            Dictionary<string, float> influences
        ) : base(name, maxDurability, normalNumber, maxAvailableNumber, "Body", tile, influences) 
        {
            
        }

        // ReSharper disable once UnusedMember.Local
        private new static UnitPreset Of(JObject jLimb)
        {
            ImmutableTile tile = null;
            if (jLimb.ContainsKey("tile"))
                tile = ImmutableTile.Of(jLimb["tile"]);

            return new LimbPreset(
                jLimb["name"].Value<string>(),
                jLimb["maxDurability"].Value<float>(),
                jLimb["normalNumber"].Value<int>(),
                jLimb["maxAvailableNumber"].Value<int>(),
                tile,
                Utils.extractDictionary<string, float>(jLimb["influences"] as JObject, s => s)
            );
        }
    }
}