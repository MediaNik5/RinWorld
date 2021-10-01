namespace RinWorld.Util.Data
{
    public class Mod
    {
        public readonly string Name;
        /// <summary>
        /// Mods having same priority happen are executed in undefined way.
        /// Mods having little priority are guaranteed to be processed earlier than once having higher priority
        /// </summary>
        public readonly int Priority;
        public Mod(string name, int priority)
        {
            Name = name;
            Priority = priority;
        }
    }
}