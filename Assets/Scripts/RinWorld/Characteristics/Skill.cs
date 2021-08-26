using RinWorld.Util.Data;

namespace RinWorld.Characteristics
{
    public class Skill
    {
        private string id;
        private int level;

        public override string ToString()
        {
            return DataHolder.Translate(id) + $": {level}\n\n" + DataHolder.Translate(id + " description");
        }
    }
}