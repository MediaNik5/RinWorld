using System.IO;
using Newtonsoft.Json.Linq;
using RinWorld.Util.IO;

namespace RinWorld.Util.Data.ModActions
{
    public class Translations : ModAction
    {
        public string Name => GetType().Name;

        public int Priority => 0;

        public void Process(DataHolder.Loader loader, Mod mod)
        {
            string path = Path.Combine(DataHolder.Loader.ModsFolder, mod.Name, DataHolder.Loader.LanguagesFolder, DataHolder.Language + ".json");
            string languageSet = Files.ReadContentsGameFolder(path);
            var jLanguageSet = JObject.Parse(languageSet);
            foreach (var jProperty in jLanguageSet)
                loader.AddTranslation(jProperty.Key, jProperty.Value.Value<string>());
        }
    }
}