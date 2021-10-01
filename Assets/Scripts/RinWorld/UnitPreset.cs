using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Linq;
using RinWorld.Control;
using RinWorld.Util.Attribute;
using RinWorld.Util.Data;
using RinWorld.Util.Data.ModActions;
using RinWorld.Util.Exception;
using Action = RinWorld.Control.Action;
using Types = RinWorld.Util.Reflection.Types;

namespace RinWorld
{
    /**
     * The minimum preset of object in game.
     */
    public class UnitPreset
    {
        // /**
        //  * Used for saving process.
        //  * Unique for every object in whole game save for its lifetime.
        //  * See id in RinWorld.Entities.Entity and RinWorld.Entities.Thing for unique identifier for operations
        //  * like creation other instances of this object.
        //  */
        public readonly Mod Mod;
        public readonly string Name;

        protected UnitPreset(string name)
        {
            Name = name;
            Mod = Mods.Current;
        }

        // /**
        //  * Initialize is being called immediately after all Unit's variables
        //  * of all Units have been loaded with respect to [Priority]
        //  * The higher the priority the earlier it gets called.
        //  */
        // [Priority]
        // public virtual void Initialize()
        // {
        //     
        // }
        //
        // [Priority]
        // public virtual void Destroy()
        // {
        // }


        // public Action[] AvailableActions(Controller controller)
        // {
        //     return Array.Empty<Action>();
        // }
        //
        // public object ExecuteAction(Action action)
        // {
        //     throw new UnsupportedActionException(this, action);
        // }

        public override string ToString()
        {
            return $"Unit {Name}";
        }

        private static readonly Dictionary<string, Func<JObject, UnitPreset>> subtypes =
            Types.GetStaticMethodsOfAllSubtypes<UnitPreset, Func<JObject, UnitPreset>>(
                "Of", 
                BindingFlags.NonPublic
            );


        public static UnitPreset Of(JObject jObject)
        {
            var type = jObject["type"].Value<string>();
            if (!subtypes.ContainsKey(type))
                throw new InvalidUnitTypeException(type);

            return subtypes[type].Invoke(jObject);
        }
    }
}