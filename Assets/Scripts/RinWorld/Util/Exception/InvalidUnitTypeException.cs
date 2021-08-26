namespace RinWorld.Util.Exception
{
    public class InvalidUnitTypeException : System.Exception
    {
        public InvalidUnitTypeException(string type) : base(
            $"Unit with name {type} is not found or doesn't have static method Of(JObject)")
        {
        }
    }
}