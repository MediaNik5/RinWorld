using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using RinWorld.Util.IO;
using UnityEngine;

namespace RinWorld.Util.Data.ModActions
{
    public class Units : ModAction
    {
        private const string UnitsFolder = "units";
        public override int Priority => 2;

        public override void Process(DataHolder.Loader loader, Mod mod, string modPath)
        {
            string folder = Path.Combine(modPath, UnitsFolder);
            JObject[] jObjects = Files.GetFilesGameFolder(folder, "*.json").Select(JObject.Parse).ToArray();

            Array.Sort(jObjects, (jObject0, jObject1) => jObject0["priority"].Value<int>() - jObject1["priority"].Value<int>());
            
            foreach (JObject file in jObjects)
                ProcessFile(loader, file);
        }
        private static void ProcessFile(DataHolder.Loader loader, JObject jUnits)
        {
            foreach (JObject jUnit in jUnits["units"])
                try
                {
                    loader.AddUnit(jUnit["name"].Value<string>(), UnitPreset.Of(jUnit));
                }
                catch (System.Exception exception)
                {
                    Debug.Log(exception);
                }

        }
    }
}