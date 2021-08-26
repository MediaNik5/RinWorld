namespace RinWorld.Control
{
    public abstract class Controller
    {
        public bool IsPlayer { get; }

        protected Controller(bool isPlayer)
        {
            IsPlayer = isPlayer;
        }

        public abstract bool HasNextAction();
        public abstract Action NextAction();
    }
}