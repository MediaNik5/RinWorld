using Newtonsoft.Json.Linq;
using RinWorld.Characteristics;
using RinWorld.Control;

namespace RinWorld.Entities
{
    public class Colonist : Entity
    {
        private Controller _controller;
        private readonly Skill[] _skills;

        protected Colonist(Controller controller) : base("colonist")
        {
            _controller = controller;
            _skills = new Skill[12];
        }

        public static Colonist Generate()
        {
            Colonist colonist = new Colonist(PlayerController.CURRENT_PLAYER);
            return colonist;
        }
    }
}