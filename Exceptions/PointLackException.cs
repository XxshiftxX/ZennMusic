using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ZennMusic.Enums;

namespace ZennMusic.Exceptions
{
    class PointLackException : Exception, IChatException
    {
        public string ChatMessage => $"잘못된 명령어 형식입니다! 명령어를 다시한번 확인해주세요!";

        public string Name { get; private set; }
        public RequestType Type { get; private set; }

        public PointLackException(string name, RequestType type)
        {
            Name = name;
            Type = type;
        }

        public PointLackException(string name, RequestType type, string message) : base(message)
        {
            Name = name;
            Type = type;
        }

        public PointLackException(string name, RequestType type, string message, Exception innerException) : base(message, innerException)
        {
            Name = name;
            Type = type;
        }

        protected PointLackException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
