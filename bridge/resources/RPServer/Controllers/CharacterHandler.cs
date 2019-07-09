﻿using System;
using GTANetworkAPI;
using RPServer.Util;
using Shared;

namespace RPServer.Controllers
{
    internal class CharacterHandler : Script
    {
        [Command("removecam")]
        public void cmd_removecam(Client client)
        {
            client.TriggerEvent("debugdestroycam");
        }

        public CharacterHandler()
        {
            AuthenticationHandler.PlayerSuccessfulLogin += PlayerSuccessfulLogin;
        }

        private void PlayerSuccessfulLogin(object source, EventArgs e)
        {
            var client = source as Client;
            if (client == null) return;
            if(!client.IsLoggedIn()) return;

            client.TriggerEvent(ServerToClient.InitCharSelection);

            var accData = client.GetAccountData();
            /*TaskManager.Run(client, async () =>
            {
                // var chars = await accData.GetCharactersAsync();
                // TODO: Send appropriate character data to client for "Rendering"
            });*/
        }

        [RemoteEvent("ApplyCharSelectionAnimation")]
        public void ClientEvent_ApplyCharSelectionAnimation(Client client) => client.PlayAnimation("missbigscore2aleadinout@ig_7_p2@bankman@", "leadout_waiting_loop", 1);
    }
}