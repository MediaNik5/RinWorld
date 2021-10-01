using System.Collections.Generic;

namespace RinWorld.Entities.Body
{
    public class BodyPart
    {
        public readonly BodyPartPreset _bodyPartPreset;
        private float _currentDurability;
        private Entity _holder;

        public BodyPart(BodyPartPreset bodyPartPreset, Entity holder)
        {
            _bodyPartPreset = bodyPartPreset;
            _currentDurability = bodyPartPreset.MaxDurability;
            _holder = holder;
        }
    }
}