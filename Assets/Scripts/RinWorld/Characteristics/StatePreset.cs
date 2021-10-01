using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RinWorld.Util;

namespace RinWorld.Characteristics
{
    public class StatePreset : UnitPreset
    {
        public static readonly StatePreset Dummy = new StatePreset("All", new Dictionary<string, float>());

        private readonly Dictionary<string, float> _influences;
        protected StatePreset(string name, IDictionary<string, float> influences) : base(name)
        {
            _influences = new Dictionary<string, float>(influences);
        }
        
        // ReSharper disable once UnusedMember.Local
        private new static UnitPreset Of(JObject jStatePreset)
        {
            return new StatePreset(
                jStatePreset["name"].Value<string>(),
                Utils.extractDictionary<string, float>(jStatePreset["influences"] as JObject, s => s)
            );
        }
    }
}