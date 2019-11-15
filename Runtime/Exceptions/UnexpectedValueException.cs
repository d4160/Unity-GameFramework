namespace d4160.Exceptions
{

    [System.Serializable]
    public class UnexpectedValueException : System.Exception
    {
        public UnexpectedValueException() { }
        public UnexpectedValueException(string message) : base(message) { }
        public UnexpectedValueException(string message, System.Exception inner) : base(message, inner) { }
        protected UnexpectedValueException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}