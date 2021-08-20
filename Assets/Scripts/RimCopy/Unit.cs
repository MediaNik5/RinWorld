using RimCopy.Attribute;

namespace RimCopy
{
    /**
     * The minimum object of game.
     */
    public class Unit : object
    {
        public readonly string guid;

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

        public override string ToString()
        {
            return $"Unit with guid {guid} and {GetType()}";
        }
    }
}