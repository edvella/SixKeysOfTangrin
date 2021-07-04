using Sentry;
using System;
using System.Runtime.Serialization;

namespace SixKeysOfTangrin
{
    [Serializable]
    public class ConnectionOutOfMapException : Exception
    {
        public ConnectionOutOfMapException() : base("Items cannot be placed out of the map")
        {
            SentrySdk.CaptureMessage("Tried to place an item outside of map");
        }

        protected ConnectionOutOfMapException(
            SerializationInfo serializationInfo,
            StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
            
        }
    }
}
