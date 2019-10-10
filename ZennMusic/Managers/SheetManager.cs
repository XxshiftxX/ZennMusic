using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Services;
using System.Collections.Generic;
using Google.Apis.Sheets.v4.Data;
using ZennMusic.Exceptions;

namespace ZennMusic.Managers
{
    class SheetManager
    {
        public static SheetsService Service { get; private set; }

        [Log]
        public static void Initialize()
        {
            var credential = GetCredential();

            Service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Zenn Bot"
            });
        }

        [Log]
        public static UserCredential GetCredential()
        {
            const string credentialPath = "token.json";

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                var secret = GoogleClientSecrets.Load(stream).Secrets;
                var scopes = new string[] { SheetsService.Scope.Spreadsheets };
                var user = "ZENNBOT";
                var token = CancellationToken.None;
                var dataStore = new FileDataStore(credentialPath, true);

                return GoogleWebAuthorizationBroker.AuthorizeAsync(secret, scopes, user, token, dataStore).Result;
            }
        }

        [Log]
        public static (int Piece, int Ticket) GetUserPoint(string name)
        {
            var sheet = GetSheet();

            var result = sheet
                .Where(x => (x[0] as string) == name)
                .FirstOrDefault()
                ?.Skip(1).Take(2)
                .Select(x => int.Parse((x as string) ?? "0"))
                .ToArray() ?? throw new UserNotFoundException(name);

            return (result[0], result[1]);
        }

        [Log]
        public static string GetUserPrefix(string name)
        {
            var sheet = GetSheet();

            var data = sheet
                .Where(x => (x[0] as string) == name)
                .FirstOrDefault();

            if (data == null || data.Count < 4)
                return null;
            return data[3] as string;
        }

        [Log]
        public static void SetPiece(string name, int piece)
        {
            var index = GetUserIndex(name);

            SetValue($"시트1!C{index + 6}", piece);
        }

        [Log]
        public static void SetTicket(string name, int ticket)
        {
            var index = GetUserIndex(name);

            SetValue($"시트1!D{index + 6}", ticket);
        }

        [Log]
        public static void SetUserPrefix(string name, string prefix)
        {
            var index = GetUserIndex(name);

            SetValue($"시트1!E{index + 6}", prefix);
        }

        [Log]
        private static IList<IList<object>> GetSheet()
        {
            const string range = "시트1!B6:E";

            var request = Service.Spreadsheets.Values.Get(ConfigManager.PieceSpreadSheetId, range);
            var response = request.Execute().Values;

            return response;
        }

        [Log]
        private static int GetUserIndex(string name)
        {
            var sheet = GetSheet();

            var result = sheet
                .Select((x, i) => (value: x, index: i))
                .Where(x => x.value[0] as string == name)
                .FirstOrDefault();

            if (result.value == null)
                throw new UserNotFoundException(name);

            return result.index;
        }

        [Log]
        private static void SetValue(string range, object value)
        {
            var body = new ValueRange
            {
                Values = new List<IList<object>>
                {
                    new List<object> { value }
                }
            };

            var setRequest = Service.Spreadsheets.Values.Update(body, ConfigManager.PieceSpreadSheetId, range);
            setRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            setRequest.Execute();
        }
    }
}
