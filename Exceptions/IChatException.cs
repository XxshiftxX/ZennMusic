using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ZennMusic.Exceptions
{
    interface IChatException
    {
        string ChatMessage { get; }
    }
}
