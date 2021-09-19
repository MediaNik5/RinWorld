using RinWorld.Util.Data;

namespace RinWorld.Characteristics
{
    public class Skill
    {
        
        private string id;
        private int level;
        public Skill(string id, int level)
        {
            this.id = id;
            this.level = level;
        }


        public override string ToString(){
            return DataHolder.Translate(id) + $": {level}\n\n" + DataHolder.Translate(id + " description");
        }
    }
}