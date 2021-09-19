using System.IO;
using Newtonsoft.Json.Linq;
using RinWorld.Util.IO;

namespace RinWorld.Util.Data.ModActions
{
    public class Units : ModAction
    {

        private const string UnitsFile = "units.json";
        public string Name => GetType().Name;
        public int Priority => 2;

        public void Process(DataHolder.Loader loader, Mod mod)
        {
            string folder = Path.Combine(DataHolder.Loader.ModsFolder, mod.Name, UnitsFile);
            string json = Files.ReadContentsGameFolder(folder);
            JArray jUnits = JArray.Parse(json);

            foreach (JObject jUnit in jUnits)
                loader.AddUnit(jUnit["name"].Value<string>(), Unit.Of(jUnit));
        }
    }
}