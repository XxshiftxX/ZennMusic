using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZennMusic.Managers
{
    static class LogManager
    {
        // NotImplementedException 쓰고싶은데 당장 테스트할때 에러나니까 그냥 빈 메서드로 선언함 ^^
        public static void Initialize() { }
        public static void Log(string message, bool hasTimestamp = true) { }
    }
}
