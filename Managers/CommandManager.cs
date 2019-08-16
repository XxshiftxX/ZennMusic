using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;
using ZennMusic.Enums;
using ZennMusic.Model;

namespace ZennMusic.Managers
{
    static class CommandManager
    {
        public static readonly Dictionary<string, Action<OnMessageReceivedArgs, string[]>> Commands =
            new Dictionary<string, Action<OnMessageReceivedArgs, string[]>>()
            {
                { "조각", GetPointCommand },
                { "지급", PayPointCommand },
                { "신청", RequestSongCommand }
            };

        private static void GetPointCommand(OnMessageReceivedArgs msgArgs, string[] cmdArgs)
        {
            var name = msgArgs.ChatMessage.DisplayName;
            var point = SheetManager.GetUserPoint(name);
            
            ChatManager.SendMessage($"[{name}] 조각 {point.Piece}개 / 티켓 {point.Ticket}장 보유중");
        }

        private static void PayPointCommand(OnMessageReceivedArgs msgArgs, string[] cmdArgs)
        {
            var type = cmdArgs[0];
            var name = cmdArgs[1];
            var point = int.Parse(cmdArgs[2] ?? "1");

            var currentPoint = SheetManager.GetUserPoint(name);

            if (type == "티켓")
                SheetManager.SetTicket(name, currentPoint.Ticket + point);
            else if (type == "조각")
                SheetManager.SetPiece(name, currentPoint.Piece + point);

            ChatManager.SendMessage($"{name}님에게 {type} {point}개를 지급하였습니다.");
        }

        private static void RequestSongCommand(OnMessageReceivedArgs msgArgs, string[] cmdArgs)
        {
            var song = string.Join(" ", cmdArgs);
            var name = msgArgs.ChatMessage.DisplayName;
            var point = SheetManager.GetUserPoint(name);

            if (point.Ticket > 0)
                SheetManager.SetTicket(name, point.Ticket - 1);
            else if (point.Piece > 2)
                SheetManager.SetPiece(name, point.Piece - 3);
            else
                return;

            SongManager.SongList.Add(new Song(song, name, point.Ticket > 0 ? RequestType.Ticket : RequestType.Piece));
            ChatManager.SendMessage($"");
        }
    }
}
