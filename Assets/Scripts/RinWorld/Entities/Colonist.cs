using RinWorld.Characteristics;
using RinWorld.Control;

namespace RinWorld.Entities
{
    public class Colonist : Entity
    {
        private Controller _controller;

        protected Colonist(Controller controller) : base("colonist")
        {
            _controller = controller;
        }

        private Skill[] skills;
    }
}