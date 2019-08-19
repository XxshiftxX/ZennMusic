using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZennMusic.Managers
{
    static class ConfigManager
    {
        private static JObject _config;

        public static string BotID { get; private set; }
        public static string BotToken { get; private set; }
        public static string[] Managers { get; private set; }
        public static string PieceSpreadSheetId { get; private set; }

        static ConfigManager()
        {
            InitializeFile();
            InitializeConfig();
        }

        private static void InitializeFile()
        {
            var curPath = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(curPath, "config.json");

            _config = JObject.Parse(File.ReadAllText(filePath));
        }

        private static void InitializeConfig()
        {
            BotID = GetStringFromConfig("BotID");
            BotToken = DecodeBase64(GetStringFromConfig("BotToken"));
            Managers = GetStringArrayFromConfig("ManagerList");
            PieceSpreadSheetId = GetStringFromConfig("PieceSpreadSheetId");
        }

        private static string GetStringFromConfig(string key) => _config[key].Value<string>();
        private static string[] GetStringArrayFromConfig(string key) => _config[key].Select(x => (string)x).ToArray();
        private static string DecodeBase64(string value) => Encoding.ASCII.GetString(Convert.FromBase64String(value));
    }
}
