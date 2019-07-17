using System;
using System.Collections;
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
    class ChatManager
    {
        private static string _channel = "qjfrntop";
        private static readonly TwitchClient _client = new TwitchClient();
        private static JObject _config;

        public static void Initialize()
        {
            InitializeConfigure();
            InitializeClient();
        }

        public static void SendMessage(string message) => _client.SendMessage(new JoinedChannel(_channel), message);

        private static void InitializeClient()
        {
            var botId = _config["BotID"].Value<string>();
            var botToken = Encoding.ASCII.GetString(Convert.FromBase64String(_config["BotToken"].Value<string>()));

            var credentials = new ConnectionCredentials(botId, botToken);
            _client.Initialize(credentials, _channel);

            _client.OnMessageReceived += OnMessageReceived;

            _client.Connect();
        }

        private static void OnMessageReceived(object sender, OnMessageReceivedArgs args)
        {
            if (!IsValidCommand(args.ChatMessage.Message))
                return;

            var command = args.ChatMessage.Message.Split().Skip(1).First();
            CommandManager.Commands[command](args, args.ChatMessage.Message.Split().Skip(2).ToArray());
        }
        
        private static bool IsValidCommand(string message)
        {
            if (!message.StartsWith("=젠 "))
                return false;
            if (!CommandManager.Commands.ContainsKey(message.Split().Skip(1).First()))
                return false;

            return true;
        }

        private static void InitializeConfigure()
        {
            var curPath = Directory.GetCurrentDirectory();
            var filePath = Path.Combine(curPath, "config.json");

            _config = JObject.Parse(File.ReadAllText(filePath));
        }
    }
}
