using RinWorld.Control;

namespace RinWorld.Util.Exception
{
    public class UnsupportedActionException : System.Exception
    {
        public UnsupportedActionException(Unit unit, Action action) : base(
            $"Action {action} is not supported by unit {unit}")
        {
        }
    }
}