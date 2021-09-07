namespace RinWorld.Control
{
    public class PlayerController : Controller
    {
        public static readonly Controller CURRENT_PLAYER = new PlayerController();


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