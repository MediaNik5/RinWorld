using RinWorld.Entities.Body;

namespace RinWorld.Characteristics
{
    public class State
    {
        private readonly StatePreset _statePreset;
        private float currentLevel = 1f;
        public State(StatePreset statePreset)
        {
            _statePreset = statePreset;
        }
    }
}