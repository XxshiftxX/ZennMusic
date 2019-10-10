using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZennMusic.Exceptions
{
    class PermissionNotFoundException : CommandException
    {
        public override string ChatMessage => "해당 명령어를 사용할 권한이 없어요!";
    }
}
