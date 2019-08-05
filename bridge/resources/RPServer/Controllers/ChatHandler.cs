using System;
using System.Text.RegularExpressions;
using GTANetworkAPI;
using RPServer.InternalAPI.Extensions;
using RPServer.Resource;
using RPServer.Util;
using RPServerClient.Chat.Util;
using Shared.Data;

namespace RPServer.Controllers
{
    internal class ChatHandler : Script
    {
        [Command(CmdStrings.CMD_OOC, Alias = CmdStrings.CMD_OOC_Alias, GreedyArg = true)]
        public void CMD_OOC(Client client, string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                SendCommandUsageText(client, "/o [Global OOC]");
                return;
            }
            message = EscapeHTML(message);
            message = RemoveColors(message);

            var playerName = string.IsNullOrEmpty(client.GetAccount().NickName) ? client.GetActiveChar().CharacterName : client.GetAccount().NickName;

            NAPI.ClientEvent.TriggerClientEventForAll(Shared.Events.ServerToClient.Chat.PushChatMessageUnfiltered, $"{Colors.COLOR_LIGHTBLUE}(( [O] {playerName}: {message} ))");
        }

        [Command(CmdStrings.CMD_B, GreedyArg = true)]
        public void CMD_B(Client client, string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                SendCommandUsageText(client, "/b [Local OOC]");
                return;
            }
            message = EscapeHTML(message);
            message = RemoveColors(message);

            foreach (var p in NAPI.Pools.GetAllPlayers())
            {
                if (client.Position.DistanceToSquared(p.Position) > Chat.NormalChatMaxDistance) continue;

                var color = GetLocalChatMessageColor(client, p, Chat.NormalChatMaxDistance);
                NAPI.ClientEvent.TriggerClientEvent(p, Shared.Events.ServerToClient.Chat.PushChatMessage, $": (( {message} ))", client.Value, color);
            }
        }

        [Command(CmdStrings.CMD_Me, GreedyArg = true)]
        public void CMD_Me(Client client, string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                SendCommandUsageText(client, "/me [action]");
                return;
            }
            message = EscapeHTML(message);
            message = RemoveColors(message);
            if (message[message.Length - 1] != '.') message += ".";

            foreach (var p in NAPI.Pools.GetAllPlayers())
            {
                if (client.Position.DistanceToSquared(p.Position) > Chat.NormalChatMaxDistance) continue;

                NAPI.ClientEvent.TriggerClientEvent(p, Shared.Events.ServerToClient.Chat.PushActionMessage, message, client.Value, Colors.COLOR_PURPLE);
            }
        }

        [Command(CmdStrings.CMD_Do, GreedyArg = true)]
        public void CMD_Do(Client client, string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                SendCommandUsageText(client, "/do [description]");
                return;
            }
            message = EscapeHTML(message);
            message = RemoveColors(message);
            if (message[message.Length - 1] != '.') message += ".";

            foreach (var p in NAPI.Pools.GetAllPlayers())
            {
                if (client.Position.DistanceToSquared(p.Position) > Chat.NormalChatMaxDistance) continue;

                NAPI.ClientEvent.TriggerClientEvent(p, Shared.Events.ServerToClient.Chat.PushDescriptionMessage, message, client.Value, Colors.COLOR_PURPLE);
            }
        }

        [RemoteEvent(Shared.Events.ClientToServer.Chat.SubmitChatMessage)]
        public void OnSubmitChatMessage(Client client, string playerText, int chatModeAsInt)
        {
            if (!client.IsLoggedIn() || !client.HasActiveChar()) return;

            var chatMode = (ChatMode)chatModeAsInt;
            playerText = EscapeHTML(playerText);
            playerText = RemoveColors(playerText);

            foreach (var p in NAPI.Pools.GetAllPlayers())
            {
                string textColor;
                switch (chatMode)
                {
                    case ChatMode.Low:
                        if (client.Position.DistanceToSquared(p.Position) > Chat.LowChatMaxDistance) continue;

                        // Add a full stop at the end of the message if needed
                        if (playerText[playerText.Length - 1] != '.') playerText += ".";

                        textColor = GetLocalChatMessageColor(client, p, Chat.LowChatMaxDistance);
                        NAPI.ClientEvent.TriggerClientEvent(p, Shared.Events.ServerToClient.Chat.PushChatMessage, $" says: {playerText}", client.Value, textColor + "[Low] ");
                        break;
                    case ChatMode.Normal:
                        if (client.Position.DistanceToSquared(p.Position) > Chat.NormalChatMaxDistance) continue;

                        // Add a full stop at the end of the message if needed
                        if (playerText[playerText.Length - 1] != '.') playerText += ".";

                        textColor = GetLocalChatMessageColor(client, p, Chat.NormalChatMaxDistance);
                        NAPI.ClientEvent.TriggerClientEvent(p, Shared.Events.ServerToClient.Chat.PushChatMessage, $" says: {playerText}", client.Value, textColor);
                        break;
                    case ChatMode.Shout:
                        if (client.Position.DistanceToSquared(p.Position) > Chat.ShoutChatMaxDistance) continue;

                        // Add an exclamation mark at the end of the message if needed
                        if (playerText[playerText.Length - 1] != '!') playerText += "!";

                        textColor = GetLocalChatMessageColor(client, p, Chat.ShoutChatMaxDistance);
                        NAPI.ClientEvent.TriggerClientEvent(p, Shared.Events.ServerToClient.Chat.PushChatMessage, $" shouts: {playerText}", client.Value, textColor);
                        break;
                    default:
                        NAPI.Util.ConsoleOutput("Error OnSubmitChatMessage while switching though chatmodes.");
                        return;
                }
            }
            Logger.GetInstance().ChatLog($"{client.GetActiveChar().CharacterName}: {playerText}");
        }

        private string GetLocalChatMessageColor(Client client, Client other, float maxDistance)
        {
            var distance = client.Position.DistanceToSquared(other.Position);

            if(distance < maxDistance / 16) return Shared.Data.Colors.COLOR_WHITE;
            if (distance < maxDistance / 8) return Shared.Data.Colors.COLOR_GRAD1;
            if (distance < maxDistance / 4) return Shared.Data.Colors.COLOR_GRAD2;
            if (distance < maxDistance / 2) return Shared.Data.Colors.COLOR_GRAD3;
            if (distance < maxDistance) return Shared.Data.Colors.COLOR_GRAD4;
            return Shared.Data.Colors.COLOR_GRAD5;
        }

        private string RemoveColors(string message)
        {
            var matches = new Regex(@"(!{#[0-9A-F]{6}})+").Matches(message);
            foreach (Match m in matches) message = message.Remove(message.IndexOf(m.Value, StringComparison.OrdinalIgnoreCase), 10);
            return message;
        }

        private static string EscapeHTML(string message)
        {
            return System.Security.SecurityElement.Escape(message);
        }

        internal static void SendCommandUsageText(Client client, string usageText)
        {
            client.TriggerEvent(Shared.Events.ServerToClient.Chat.PushChatMessageUnfiltered, EscapeHTML($"{Shared.Data.Colors.COLOR_GRAD3}[Usage]: {Shared.Data.Colors.COLOR_GRAD1}{usageText}"));
        }

        internal static void SendCommandErrorText(Client client, string errorText)
        {
            client.TriggerEvent(Shared.Events.ServerToClient.Chat.PushChatMessageUnfiltered, EscapeHTML($"{Shared.Data.Colors.COLOR_YELLOW}<!> {Shared.Data.Colors.COLOR_WHITE}{errorText}"));
        }

        internal static void SendCommandSuccessText(Client client, string text)
        {
            client.TriggerEvent(Shared.Events.ServerToClient.Chat.PushChatMessageUnfiltered, EscapeHTML($"{Shared.Data.Colors.COLOR_GREEN}<!> {Shared.Data.Colors.COLOR_WHITE}{text}"));
        }

        internal static void SendClientMessage(Client client, string message)
        {
            client.TriggerEvent(Shared.Events.ServerToClient.Chat.PushChatMessageUnfiltered, EscapeHTML(message));
        }
    }
}
