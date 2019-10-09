using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ZennMusic.Exceptions
{
    class CommandException : Exception
    {
        public string ChatMessage { get; set; }

        public CommandException(string chatMessage)
        {
            ChatMessage = chatMessage;
        }

        public CommandException(string message, string chatMessage) : base(message)
        {
            ChatMessage = chatMessage;
        }

        public CommandException(string message, string chatMessage, Exception innerException) : base(message, innerException)
        {
            ChatMessage = chatMessage;
        }
    }
}
