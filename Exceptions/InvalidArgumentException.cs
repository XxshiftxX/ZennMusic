using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZennMusic.Exceptions
{
    class InvalidArgumentException : CommandException
    {
        public override string ChatMessage => $"잘못된 명령어 형식이에요! {_reason}";

        private string _reason = string.Empty;

        public InvalidArgumentException() { }
        public InvalidArgumentException(string reason)
        {
            _reason = reason;
        }
    }
}
