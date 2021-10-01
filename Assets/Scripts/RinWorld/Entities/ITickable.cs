namespace RinWorld.Entities
{
    public interface ITickable
    {
        /// <summary>
        /// Ticks gone since object creation
        /// </summary>
        public ulong LifeTime { get; }
        
        /// <summary>
        /// Proceeds tick by number <c>tick</c>, but it does not mean,
        /// that after this function LifeTime would be equal to tick.
        /// 
        /// LifeTime after this function returns will be increased.
        /// </summary>
        /// <param name="tick">Global tick number</param>
        public void Tick(ulong tick);
    }
}