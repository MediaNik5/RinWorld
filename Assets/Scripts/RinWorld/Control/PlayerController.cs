namespace RinWorld.Control
{
    public class PlayerController : Controller
    {
        public PlayerController() : base(true)
        {
        }

        public override bool HasNextAction()
        {
            return false;
        }

        public override Action NextAction()
        {
            return null;
        }
    }
}