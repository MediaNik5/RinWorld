namespace RinWorld.Entities
{
    public class Entity : Unit
    {
        private string _name;

        protected Entity(string name)
        {
            _name = name;
        }
    }
}