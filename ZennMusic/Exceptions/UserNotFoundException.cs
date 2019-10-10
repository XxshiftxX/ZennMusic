using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZennMusic.Exceptions
{
    class UserNotFoundException : CommandException
    {
        public override string ChatMessage => $"신청곡 조각 시트에 {_username}님의 이름이 존재하지 않아요!";

        private string _username;

        public UserNotFoundException(string username)
        {
            _username = username;
        }
    }
}
