using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using ZennMusic.Exceptions;

namespace ZennMusic.Managers
{
    class ChatManager
    {
        private static string _channel = "qjfrntop";
        private static readonly TwitchClient _client = new TwitchClient();

        public static void Initialize()
        {
            InitializeClient();
        }

        public static void SendMessage(string message) => _client.SendMessage(new JoinedChannel(_channel), message);

        private static void InitializeClient()
        {
            var botId = ConfigManager.BotID;
            var botToken = ConfigManager.BotToken;

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
            Action commandAction = () => CommandManager.Commands[command](args, args.ChatMessage.Message.Split().Skip(2).ToArray());

            ExecuteCommand(commandAction);
        }

        private static void ExecuteCommand(Action action)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(action);
            }
	        catch (CommandException exception)
	        {
                SendMessage(exception.ChatMessage);
	        }
        }
        
        private static bool IsValidCommand(string message)
        {
            if (!message.StartsWith("=젠 "))
                return false;
            if (!CommandManager.Commands.ContainsKey(message.Split().Skip(1).First()))
                return false;

            return true;
        }
    }
}
