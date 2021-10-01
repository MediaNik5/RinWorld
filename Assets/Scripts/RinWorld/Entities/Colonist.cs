using System;
using System.Collections.Generic;
using System.Linq;
using RinWorld.Characteristics;
using RinWorld.Control;
using RinWorld.Entities.Body;
using RinWorld.Util.Data;
using RinWorld.Worlds.Generator;

namespace RinWorld.Entities
{
    public class Colonist : Entity
    {
        private Controller _controller;
        private readonly Dictionary<string, State> _states = new Dictionary<string, State>();
        private readonly Skill[] _skills = new Skill[12];
        public const int SkillNumber = 12;

        protected Colonist(Controller controller) : base("colonist")
        {
            _controller = controller;
        }

        public static Colonist Generate(int seed)
        {
            var colonist = new Colonist(PlayerController.CURRENT_PLAYER);
            colonist.GenerateSkills(seed);
            colonist.GenerateBodyParts(seed);
            colonist.GenerateStates(seed);
            return colonist;
        }

        protected void GenerateSkills(int seed)
        {
            Wave[] waves = Noise.GenerateWaves(new Random(seed), 2, 1f);
            float[,] skills = Noise.GenerateNormalized(SkillNumber, 1, waves, normalizationValue: SkillNumber * 8);
            var skillPresets = DataHolder.GetUnits<SkillPreset>();
            for (int i = 0; i < skills.Length; i++)
                _skills[i] = new Skill(skillPresets[i], (int) (skills[i, 0] + 0.5f));
        }

        protected void GenerateBodyParts(int seed)
        {
            var bodyPartPresets = DataHolder.GetUnits<BodyPartPreset>();
            foreach (BodyPartPreset preset in bodyPartPresets.Where(preset => preset.NormalNumber != 0))
                bodyParts.Add(preset.Name, new BodyPart(preset, this));
            
        }
        
        protected void GenerateStates(int seed)
        {
            var statePresets = DataHolder.GetUnits<StatePreset>();
            foreach (var preset in statePresets)
                _states.Add(preset.Name, new State(preset));
        }
    }
}











