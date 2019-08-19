using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZennMusic.Managers
{
    public static class LogManager
    {
        private static FileStream _logger;

        private string LogFileDirectory => Path.Combine(Directory.GetCurrentDirectory(), "Log");
        private static string CurrentTimeString => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");

        public static void Initialize() 
        {
            var dateString = DateTime.Now.ToString("yyyy-MM-dd");
            var LogFilePath = $@"{LogFileDirectory}\ZennLog {dateString}.txt";

            if (!Directory.Exists(LogFileDirectory))
                Directory.CreateDirectory(LogDirectory);
            if (!File.Exists(LogFilePath))
                File.Create(LogFilePath).Close();

            _logger = File.Open(LogFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            
            Log("[Logger Started]");
        }
        public static void Log(string message, bool hasTimestamp = true) 
        {
            if (hasTimestamp)
                message += $"[{CurrentTimeString}] ";
            
            var messageBytes = Encoding.UTF8.GetBytes(message + "\n");
            _logger.Write(messageBytes, 0, messageBytes.Length);
            _logger.Flush();
        }
    }

    public class InterceptorAttribute : Attribute, IMethodDecorator
    {
        public MethodBase Method;
        public object[] Arguments;
        public string ArgumentString => string.Join(", ", Arguments);

        public void Init(object instance, MethodBase method, object[] args)
        {
            Method = method;
            Arguments = args;
        }

        public void OnEntry()
            => LogManager.Log($"[Method Entry] {Method.Name}[{GetHashCode()}] >> ({ArgumentString})");

        public void OnExit()
            => LogManager.Log($"[Method Exit]  {Method.Name}[{GetHashCode()}]");
    }
}
