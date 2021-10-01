using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RinWorld.Util.Reflection;

namespace RinWorld.Util.Data.ModActions
{
    
    public abstract class ModAction
    {
        public string Name => GetType().Name;
        /// <summary>
        /// ModActions having same priority happen are executed in undefined way
        /// ModActions having little priority are guaranteed to be processed earlier than once having higher priority
        /// </summary>
        public abstract int Priority { get; }
        public abstract void Process(DataHolder.Loader loader, Mod mod, string modPath);
        
    }

    public static class Mods
    {
        public static Mod Current { get; private set; }
        private static readonly Dictionary<string, ModAction> modActions;
        static Mods() => 
            modActions = Types.GetInstancesOfAllSubtypes<ModAction>();

        internal static void Process(DataHolder.Loader loader, Mod[] mods)
        {
            var modActionsSorted = modActions.Values.ToArray();
            Array.Sort(mods, (mod0, mod1) => mod1.Priority - mod0.Priority);
            Array.Sort(modActionsSorted, (action0, action1) => action0.Priority - action1.Priority);

            foreach (var modAction in modActionsSorted)
            {
                foreach (var mod in mods)
                {
                    Current = mod;
                    modAction.Process(loader, mod, Path.Combine(DataHolder.Loader.ModsFolder, mod.Name));
                }
            }

            Current = null;
        }
        public static void ProcessAction<TAction>(DataHolder.Loader loader, Mod[] mods) where TAction : ModAction=> 
            ProcessAction(loader, mods, typeof(TAction).Name);
        
        public static void ProcessAction(DataHolder.Loader loader, Mod[] mods, string name)
        {
            foreach (Mod mod in mods)
                modActions[name].Process(loader, mod, Path.Combine(DataHolder.Loader.ModsFolder, mod.Name));
        }
    }
}