namespace TutorHelper.Middlewares.Exceptions
{

    internal class ForbidException : Exception
    {
        public ForbidException()
        {
        }

        public ForbidException(string message) : base(message)
        {
        }

        public ForbidException(string message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}