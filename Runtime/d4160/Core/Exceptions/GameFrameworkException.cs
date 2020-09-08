using System;

namespace d4160.Core.Exceptions
{

    [Serializable]
    public class GameFrameworkException : SystemException
    {
        private const int Result = -2031467261;

        public GameFrameworkException() : base("A Game Framework error occurred!")
        {
            HResult = Result;
        }

        public GameFrameworkException(string message) : base(message)
        {
            HResult = Result;
        }

        public GameFrameworkException(string message, Exception inner) : base(message, inner)
        {
            HResult = Result;
        }
        
        protected GameFrameworkException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}