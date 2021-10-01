using RinWorld.Util.Data;

namespace RinWorld.Characteristics
{
    public class Skill
    {
        
        private readonly SkillPreset id;
        private int level;
        public Skill(SkillPreset id, int level)
        {
            this.id = id;
            this.level = level;
        }


        public override string ToString(){
            return DataHolder.Translate(id.ToString()) + $": {level}\n\n" + DataHolder.Translate(id + " description");
        }
    }
}