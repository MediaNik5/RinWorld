using System.IO;
using Newtonsoft.Json.Linq;
using RinWorld.Util.IO;

namespace RinWorld.Util.Data.ModActions
{
    public class Translations : ModAction
    {
        public override int Priority => 0;
        public override void Process(DataHolder.Loader loader, Mod mod, string modPath)
        {
            string path = Path.Combine(modPath, DataHolder.Loader.LanguagesFolder, DataHolder.Language + ".json");
            string languageSet = Files.ReadContentsGameFolder(path);
            var jLanguageSet = JObject.Parse(languageSet);
            loader.ResetTranslation();
            foreach (var jProperty in jLanguageSet)
                loader.AddTranslation(jProperty.Key, jProperty.Value.Value<string>());
        }
    }
}