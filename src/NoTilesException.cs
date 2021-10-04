using System;
using System.Runtime.Serialization;

namespace MahjongSharp {
    public class NoTilesException : Exception {
        public NoTilesException(string message) : base(message) { }
        public NoTilesException(string message, Exception inner) : base(message, inner) { }
        protected NoTilesException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
