using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace ZennMusic.Managers
{
    class CommandRouter
    {
        public static readonly Dictionary<string, Action<OnMessageReceivedArgs, string[]>> Commands = 
            new Dictionary<string, Action<OnMessageReceivedArgs, string[]>>()
            {

            };

        private static readonly TwitchClient _client = new TwitchClient();
        private static JObject config;

        public static void Initialize()
        {
            InitializeConfigure();
            InitializeClient();
        }

        private static void InitializeClient()
        {
            var botId = config["BotID"].Value<string>();
            var botToken = Encoding.ASCII.GetString(Convert.FromBase64String(config["BotToken"].Value<string>()));

            var credentials = new ConnectionCredentials(botId, botToken);
            _client.Initialize(credentials, "producerzenn");

            _client.OnMessageReceived += OnMessageReceived;

            _client.Connect();
        }

        private static void OnMessageReceived(object sender, OnMessageReceivedArgs args)
        {
            Debug.WriteLine($"MSG:  {args.ChatMessage.Message}");
            if (!IsValidCommand(args.ChatMessage.Message))
                return;

            var command = args.ChatMessage.Message.Split()[1];
            Commands[command](args, args.ChatMessage.Message.Split().Skip(1).ToArray());
        }
        
        private static bool IsValidCommand(string message)
        {
            if (!message.StartsWith("!"))
                return false;
            if (!Commands.ContainsKey(new string(message.Split()[0].Skip(1).ToArray())))
                return false;

            return true;
        }

        private static void InitializeConfigure()
        {
            var curPath = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(curPath, "config.json");

            config = JObject.Parse(File.ReadAllText(filePath));
        }
    }
}
