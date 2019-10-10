using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZennMusic.Exceptions
{
    class LackOfPointException : CommandException
    {
        public override string ChatMessage => "포인트가 부족해요! =젠 조각 명령어로 보유 포인트를 확인해주세요~";
    }
}
