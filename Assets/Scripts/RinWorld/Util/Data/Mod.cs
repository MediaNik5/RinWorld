using System;
using System.Collections.Generic;
using System.Linq;
using RinWorld.Util.Data.ModActions;

namespace RinWorld.Util.Data
{
    public class Mod
    {
        public readonly string Name;
        /// <summary>
        /// Mods having same priority happen are executed in undefined way.
        /// Mods having little priority are guaranteed to be processed earlier than once having higher priority
        /// </summary>
        private readonly int _priority;
        public Mod(string name, int priority)
        {
            Name = name;
            _priority = priority;
        }
        
        private static readonly Dictionary<string, ModAction> modActions;
        static Mod()
        {
            var types = Types.GetAllSubtypes<ModAction>();
            modActions = new Dictionary<string, ModAction>(types.Length);
            for (int i = 0; i < types.Length; i++)
            {
                var modAction = (ModAction) Activator.CreateInstance(types[i], true);
                modActions[modAction.Name] = modAction;
            }
        }
        public static void Process(DataHolder.Loader loader, Mod[] mods)
        {
            var modActionsSorted = modActions.Values.ToArray();
            Array.Sort(modActionsSorted, (action0, action1) => action0.Priority - action1.Priority);
            foreach (var modAction in modActionsSorted)
                foreach (var mod in mods)
                    modAction.Process(loader, mod);
        }
        public static void ProcessAction<Action>(DataHolder.Loader loader, Mod[] mods)
        {
            ProcessAction(loader, mods, typeof(Action).Name);
        }
        public static void ProcessAction(DataHolder.Loader loader, Mod[] mods, string name)
        {
            foreach (Mod mod in mods)
                modActions[name].Process(loader, mod);
        }
    }
}