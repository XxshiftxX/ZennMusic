using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Services;
using System.Collections.Generic;
using Google.Apis.Sheets.v4.Data;

namespace ZennMusic.Managers
{
    class SheetManager
    {
        public static string PieceSpreadSheetId = "1fndP3ddyqehCIn6vcpEiZOOixzYN6MX8puCnLdOIqgM";


        public static SheetsService Service { get; private set; }


        public static void Initialize()
        {
            var credential = GetCredential();

            Service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Zenn Bot"
            });
        }

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

        public static (int Piece, int Ticket) GetUserPoint(string name)
        {
            var sheet = GetSheet();

            var result = sheet
                .Where(x => (x[0] as string) == name)
                .FirstOrDefault()
                .Skip(1)
                .Select(x => int.Parse((x as string) ?? "0"))
                .ToArray();

            return (result[0], result[1]);
        }

        public static void SetPiece(string name, int piece)
        {
            var index = GetUserIndex(name);

            SetValue($"시트1!C{index + 6}", piece);
        }

        public static void SetTicket(string name, int ticket)
        {
            var index = GetUserIndex(name);

            SetValue($"시트1!D{index + 6}", ticket);
        }

        private static IList<IList<object>> GetSheet()
        {
            const string range = "시트1!B6:E";

            var request = Service.Spreadsheets.Values.Get(PieceSpreadSheetId, range);
            var response = request.Execute().Values;

            return response;
        }

        private static int GetUserIndex(string name)
        {
            var sheet = GetSheet();

            var result = sheet
                .Select((x, i) => (value: x, index: i))
                .Where(x => x.Item1[0] as string == name)
                .FirstOrDefault();

            return result.index;
        }

        private static void SetValue(string range, object value)
        {
            var body = new ValueRange
            {
                Values = new List<IList<object>>
                {
                    new List<object> { value }
                }
            };

            var setRequest = Service.Spreadsheets.Values.Update(body, PieceSpreadSheetId, range);
            setRequest.ValueInputOption =
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            setRequest.Execute();
        }
    }
}
