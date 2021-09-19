using System.IO;
using Newtonsoft.Json.Linq;
using RinWorld.Util.IO;

namespace RinWorld.Util.Data.ModActions
{
    public class Conditions : ModAction
    {

        private const string ConditionsFile = "conditions.json";
        public string Name => GetType().Name;
        public int Priority => 4;
        public void Process(DataHolder.Loader loader, Mod mod)
        {
            string path = Path.Combine(DataHolder.Loader.ModsFolder, mod.Name, ConditionsFile);
            string json = Files.ReadContentsGameFolder(path);
            var jConditions = JObject.Parse(json);

            foreach (var jCondition in jConditions)
                loader.AddCondition(jCondition.Key, jCondition.Value.Value<float>());
        }
    }
}