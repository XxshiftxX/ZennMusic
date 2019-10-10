using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ZennMusic.Exceptions
{
    public class InvalidCommandArgumentException : Exception, IChatException
    {
        public string ChatMessage => "잘못된 명령어 형식입니다! 명령어를 다시한번 확인해주세요!";
        public string Chat { get; private set; }

        public InvalidCommandArgumentException(string chat)
        {
            Chat = chat;
        }

        public InvalidCommandArgumentException(string chat, string message) : base(message)
        {
            Chat = chat;
        }

        public InvalidCommandArgumentException(string chat, string message, Exception innerException) : base(message, innerException)
        {
            Chat = chat;
        }

        protected InvalidCommandArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
