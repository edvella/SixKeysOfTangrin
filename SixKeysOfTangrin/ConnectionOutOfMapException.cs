using System;
using System.Runtime.Serialization;

namespace SixKeysOfTangrin
{
    [Serializable]
    public class ConnectionOutOfMapException : Exception
    {
        public ConnectionOutOfMapException() : base("Items cannot be placed out of the map")
        {
        }

        protected ConnectionOutOfMapException(
            SerializationInfo serializationInfo,
            StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
            
        }
    }
}
