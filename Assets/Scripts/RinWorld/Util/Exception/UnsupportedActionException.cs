using RinWorld.Control;

namespace RinWorld.Util.Exception
{
    public class UnsupportedActionException : System.Exception
    {
        public UnsupportedActionException(UnitPreset unitPreset, Action action) : base(
            $"Action {action} is not supported by unit {unitPreset}")
        {
        }
    }
}