using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using RinWorld.Control;
using RinWorld.Util.Attribute;
using RinWorld.Util.Exception;
using Action = RinWorld.Control.Action;

namespace RinWorld
{
    /**
     * The minimum object of game.
     */
    public class Unit
    {
        /**
         * Used for saving process.
         * Unique for every object in whole game save for its lifetime.
         * See id in RinWorld.Entities.Entity and RinWorld.Entities.Thing for unique identifier for operations
         * like creation other instances of this object.
         */
        public readonly string guid;

        protected Unit()
        {
            guid = Guid.NewGuid().ToString();
        }

        /**
         * Initialize is being called immediately after all Unit's variables
         * of all Units have been loaded with respect to [Priority]
         * The higher the priority the earlier it gets called.
         */
        [Priority]
        public virtual void Initialize()
        {
            
        }

        [Priority]
        public virtual void Destroy()
        {
        }


        public Action[] AvailableActions(Controller controller)
        {
            return Array.Empty<Action>();
        }

        public object ExecuteAction(Action action)
        {
            throw new UnsupportedActionException(this, action);
        }

        public override string ToString()
        {
            return $"Unit with guid {guid} and of type {GetType().Name}";
        }

        private static readonly Dictionary<string, Func<JObject, Unit>> creators;

        static Unit()
        {
            var types =
                (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                    from type in assembly.GetTypes()
                    where type.IsSubclassOf(typeof(Unit))
                    select type).ToArray();
            creators = new Dictionary<string, Func<JObject, Unit>>(types.Length);
            foreach (var type in types)
            {
                var method = type.GetMethod("Of", BindingFlags.Static | BindingFlags.NonPublic);

                if (method != null)
                    creators.Add(
                        type.Name,
                        Delegate.CreateDelegate(typeof(Func<JObject, Unit>), null, method) as Func<JObject, Unit>
                    );
            }
        }

        public static Unit Of(JObject jObject)
        {
            var type = jObject["type"].Value<string>();
            if (!creators.ContainsKey(type))
                throw new InvalidUnitTypeException(type);

            return creators[type].Invoke(jObject);
        }
    }
}