using System;
using System.IO;
using Newtonsoft.Json.Linq;
using RinWorld.Util.IO;
using UnityEngine;

namespace RinWorld.Util.Data.ModActions
{
    public class Keystrokes : ModAction
    {
        private static readonly string KeysFile = Path.Combine(DataHolder.Loader.UtilFolder,  "keys.json");
        public override int Priority => 3;

        public override void Process(DataHolder.Loader loader, Mod mod, string modPath)
        {
            string path = Path.Combine(modPath, KeysFile);
            string json = Files.ReadContentsGameFolder(path);
            var jObject = JObject.Parse(json);

            foreach (var jProperty in jObject)
            {
                var parsed = Enum.TryParse(jProperty.Value.Value<string>(), out KeyCode keyCode);
                if (!parsed)
                    throw new System.Exception($"Could not load key {jProperty.Key} for {jProperty.Value}");
                loader.AddKey(jProperty.Key, keyCode);
            }
        }
    }
}