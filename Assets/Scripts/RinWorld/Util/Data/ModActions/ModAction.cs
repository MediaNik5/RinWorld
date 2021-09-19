using System;

namespace RinWorld.Util.Data.ModActions
{
    
    public interface ModAction
    {
        public string Name { get; }
        /// <summary>
        /// ModActions having same priority happen are executed in undefined way
        /// ModActions having little priority are guaranteed to be processed earlier than once having higher priority
        /// </summary>
        public int Priority { get; }
        public void Process(DataHolder.Loader loader, Mod mod);
    }
}