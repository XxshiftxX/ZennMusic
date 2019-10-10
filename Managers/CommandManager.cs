using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client.Events;
using ZennMusic.Enums;
using ZennMusic.Exceptions;
using ZennMusic.Models;

namespace ZennMusic.Managers
{
    static class CommandManager
    {
        public static readonly Dictionary<string, Action<OnMessageReceivedArgs, string[]>> Commands =
            new Dictionary<string, Action<OnMessageReceivedArgs, string[]>>()
            {
                { "조각", GetPointCommand },
                { "지급", PayPointCommand },
                { "신청", RequestSongCommand },
                { "칭호", SetPrefixCommand },
                { "주사위", RollDiceCommand }
            };

        [Log]
        private static void RollDiceCommand(OnMessageReceivedArgs msgArgs, string[] cmdArgs)
        {
            if (cmdArgs.Length != 0 && cmdArgs.Length != 2)
                throw new InvalidArgumentException();

            var start = 1;
            var end = 6;

            if (cmdArgs.Length == 2)
            {
                var parseStart = int.TryParse(cmdArgs[0], out start);
                var parseEnd = int.TryParse(cmdArgs[1], out end);
                if (!parseStart || !parseEnd)
                    throw new InvalidArgumentException("범위는 숫자로 입력해주세요!");
            }

            ChatManager.SendMessage($"{msgArgs.ChatMessage.DisplayName}님의 결과는 [ {new Random().Next(start, end + 1)} ] 입니다!");
        }

        [Log]
        private static void SetPrefixCommand(OnMessageReceivedArgs msgArgs, string[] cmdArgs)
        {
            if (!ConfigManager.Managers.Contains(msgArgs.ChatMessage.Username))
                throw new PermissionNotFoundException();
            if (cmdArgs.Length < 2)
                throw new InvalidArgumentException();

            var name = cmdArgs[0];
            var prefix = cmdArgs[1];

            SheetManager.SetUserPrefix(name, prefix);
            ChatManager.SendMessage($"{name}님의 칭호를 [{prefix}]로 설정했어요!");
        }

        [Log]
        private static void GetPointCommand(OnMessageReceivedArgs msgArgs, string[] cmdArgs)
        {
            var name = msgArgs.ChatMessage.DisplayName;
            var point = SheetManager.GetUserPoint(name);
            var prefix = SheetManager.GetUserPrefix(name);

            ChatManager.SendMessage($"{(string.IsNullOrEmpty(prefix) ? string.Empty : $"[{prefix}]")} {name} 조각 {point.Piece}개 / 티켓 {point.Ticket}장 보유중");
        }

        [Log]
        private static void PayPointCommand(OnMessageReceivedArgs msgArgs, string[] cmdArgs)
        {
            if (!ConfigManager.Managers.Contains(msgArgs.ChatMessage.Username))
                throw new PermissionNotFoundException();
            if (cmdArgs.Length < 2)
                throw new InvalidArgumentException();

            var type = cmdArgs[0];
            var name = cmdArgs[1];
            var point = GetDefaultPoint(cmdArgs);

            var currentPoint = SheetManager.GetUserPoint(name);

            if (type == "티켓")
                SheetManager.SetTicket(name, currentPoint.Ticket + point);
            else if (type == "조각")
                SheetManager.SetPiece(name, currentPoint.Piece + point);

            ChatManager.SendMessage($"{name}님에게 {type} {point}개를 지급하였습니다.");
        }

        private static int GetDefaultPoint(string[] cmdArgs)
        {
            int point = 1;
            if (cmdArgs.Length > 2)
            {
                var parseResult = int.TryParse(cmdArgs[2], out point);
                if (!parseResult)
                    throw new InvalidArgumentException("갯수는 숫자로 입력해주세요!");
            }

            return point;
        }

        [Log]
        private static void RequestSongCommand(OnMessageReceivedArgs msgArgs, string[] cmdArgs)
        {
            var song = string.Join(" ", cmdArgs);
            var name = msgArgs.ChatMessage.DisplayName;

            if (isCooltime(name))
                throw new RequestCooltimeException();

            var point = SheetManager.GetUserPoint(name);

            if (point.Ticket > 0)
                SheetManager.SetTicket(name, point.Ticket - 1);
            else if (point.Piece > 2)
                SheetManager.SetPiece(name, point.Piece - 3);
            else
                throw new LackOfPointException();

            var type = point.Ticket > 0 ? RequestType.Ticket : RequestType.Piece;

            SongManager.SongList.Add(new Song(song, name, type));
            ChatManager.SendMessage($"{name}님의 {GetNameOfRequestType(type)} {(type == RequestType.Ticket ? "1장" : "3개")}를 사용하여 곡을 신청했어요");
        }

        private static string GetNameOfRequestType(RequestType type)
        {
            switch (type)
            {
                case RequestType.Piece:
                    return "조각";
                case RequestType.Ticket:
                    return "티켓";
                case RequestType.Special:
                    return string.Empty;
                default:
                    throw new ArgumentException();
            }
        }

        private static bool isCooltime(string user)
            => SongManager.RemovedSongList
                .Concat(SongManager.SongList)
                .Reverse()
                .Take(4)
                .Any(song => song.Requester == user);
    }
}
