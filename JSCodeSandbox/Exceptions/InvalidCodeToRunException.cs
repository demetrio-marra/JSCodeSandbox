namespace JSCodeSandbox.Exceptions
{
    public class InvalidCodeToRunException : ValidationException
    {
        public InvalidCodeToRunException(string message) : base(message)
        {
        }
    }
}
