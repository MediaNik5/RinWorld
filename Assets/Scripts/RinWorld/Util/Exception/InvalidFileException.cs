namespace RinWorld.Util.Exception
{
    public class InvalidFileException : System.Exception
    {
        public InvalidFileException(string path, System.Exception cause)
            : base($"Invalid file on {path}", cause)
        {
        }
    }
}