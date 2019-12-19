using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZennMusic.Exceptions
{
    class RequestDisabledException : CommandException
    {
        public override string ChatMessage => "현재 신청곡을 받고있지 않아요!";
    }
}
