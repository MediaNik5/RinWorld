using System;
using RinWorld.Characteristics;
using RinWorld.Control;
using RinWorld.World.Generator;

namespace RinWorld.Entities
{
    public class Colonist : Entity
    {
        private Controller _controller;
        private readonly Skill[] _skills;
        public const int SkillNumber = 12;

        protected Colonist(Controller controller) : base("colonist")
        {
            _controller = controller;
            _skills = new Skill[12];
        }

        public static Colonist Generate(int seed)
        {
            var colonist = new Colonist(PlayerController.CURRENT_PLAYER);
            Wave[] waves = Noise.GenerateWaves(new Random(seed), 2, 1f);
            float[,] skills = Noise.GenerateNormalized(SkillNumber, 1, waves, normalizationValue: SkillNumber * 8);
            for (int i = 0; i < skills.Length; i++)
            {
                colonist._skills[i] = new Skill("", (int)(skills[i, 0] + 0.5f));
            }
            return colonist;
        }
    }
}