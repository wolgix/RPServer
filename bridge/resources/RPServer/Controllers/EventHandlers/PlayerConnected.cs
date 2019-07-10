﻿using GTANetworkAPI;
using RPServer.Resource;
using RPServer.Util;

namespace RPServer.Controllers.EventHandlers
{
    internal class PlayerConnected : Script
    {
        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Client client)
        {
            client.SendChatMessage(AccountStrings.InfoWelcome);
            AuthenticationHandler.SetLoginState(client, true);
            Logger.GetInstance().AuthLog($"Player (name: {client.Name}, social: {client.SocialClubName}, IP: {client.Address}) has connected to the server.");
            client.TriggerEvent("GetVersion", $"{Game.Globals.SERVER_NAME}-{Game.Globals.VERSION}");

            // Init the Action Queue for the Task Manager
            client.InitActionQueue();

        }
    }
}