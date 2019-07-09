using System;
using System.Threading;
using GTANetworkAPI;
using RPServer.Database;
using RPServer.Models;
using RPServer.Util;
using Shared;

namespace RPServer.Game
{
    internal class Initialization : Script
    {
        public const string SERVER_NAME = "AlphaRP";
#if DEBUG
        public const string VERSION = ThisAssembly.Git.Tag + " on " + ThisAssembly.Git.Branch;
#else
        public const string VERSION = ThisAssembly.Git.BaseTag;
#endif

        public static readonly Vector3 DefaultSpawnPos = new Vector3(-782.1527709960938f, 19.77294921875f, 41.93227767944336f);
        private static Timer _expiredEmailTokensTimer;

        [ServerEvent(Event.ResourceStart)]
        public async void OnResourceStart()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\n\n---------------------------- STARTING {SERVER_NAME} ({VERSION}) ----------------------------");
            Console.ResetColor();
            Console.WriteLine();

            // Server Settings
            NAPI.Server.SetAutoSpawnOnConnect(false);
            NAPI.Server.SetAutoRespawnAfterDeath(false);
            NAPI.Server.SetDefaultSpawnLocation(DefaultSpawnPos);
            NAPI.Server.SetGlobalServerChat(false);

            // Sever World Settings
            NAPI.World.SetTime(0, 0, 0);
            NAPI.World.ResetIplList();

            // Initialize the Logger 
            Logger.GetInstance();

            // Get Database Settings (meta.xml)
            DbConnectionProvider.ProviderName = NAPI.Resource.GetSetting<string>(this, "DB_PROVIDER");

            var dbConnectionStringBuilder = DbConnectionProvider.CreateDbConnectionStringBuilder();
            dbConnectionStringBuilder.Add("Server", NAPI.Resource.GetSetting<string>(this, "DB_HOST"));
            dbConnectionStringBuilder.Add("Port", NAPI.Resource.GetSetting<uint>(this, "DB_PORT"));
            dbConnectionStringBuilder.Add("Database", NAPI.Resource.GetSetting<string>(this, "DB_DATABASE"));
            dbConnectionStringBuilder.Add("UserID", NAPI.Resource.GetSetting<string>(this, "DB_USERNAME"));
            dbConnectionStringBuilder.Add("Password", NAPI.Resource.GetSetting<string>(this, "DB_PASSWORD"));
            dbConnectionStringBuilder.Add("ConvertZeroDateTime", true);
            DbConnectionProvider.ConnectionString = dbConnectionStringBuilder.ConnectionString;

            // Test SQL Connection
            await DbConnectionProvider.TestConnection();

            // Get SMTP Settings (meta.xml)
            EmailSender.SmtpHost = NAPI.Resource.GetSetting<string>(this, "SMTP_HOST");
            EmailSender.SmtpPort = NAPI.Resource.GetSetting<int>(this, "SMTP_PORT");
            EmailSender.SmtpUsername = NAPI.Resource.GetSetting<string>(this, "SMTP_USERNAME");
            EmailSender.SmtpPassword = NAPI.Resource.GetSetting<string>(this, "SMTP_PASSWORD");
            // Have expired tokens get removed once per hour
            _expiredEmailTokensTimer = new Timer(OnRemoveExpiredEmailTokens, null, 1, Timeout.Infinite);
        }

        [RemoteEvent(ClientToServer.SubmitPlayerCommand)]
        public void ClientEvent_OnPlayerCommand(Client client, string cmd)
        { // This CANNOT block commands
            var username = "UNREGISTERED";
            if (client.IsLoggedIn())
            {
                var accData = client.GetAccountData();
                username = accData.Username;
            }
            Logger.GetInstance().CommandLog(username, cmd);
        }

        private async void OnRemoveExpiredEmailTokens(object state)
        {
            await EmailToken.RemoveExpiredCodesAsync();
            Logger.GetInstance().ServerInfo("Removing expired email verification tokens from the database.");
            _expiredEmailTokensTimer.Change(1000 * 60 * 60, Timeout.Infinite);

        }
    }
}