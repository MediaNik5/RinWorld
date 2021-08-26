namespace RinWorld.Util.Exception
{
    public class InvalidStateException : System.Exception
    {
        public InvalidStateException()
        {
        }

        public InvalidStateException(string s) : base(s)
        {
        }
    }
}