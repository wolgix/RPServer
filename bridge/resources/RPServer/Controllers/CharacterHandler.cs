﻿using System;
using System.Collections.Generic;
using GTANetworkAPI;
using Newtonsoft.Json;
using RPServer.Models;
using RPServer.Resource;
using RPServer.Util;
using Shared;

namespace RPServer.Controllers
{
    internal class CharacterHandler : Script
    {
        [Command("removecam")]
        public void cmd_removecam(Client client)
        {
            client.TriggerEvent(ServerToClient.EndCharSelection);
        }

        [Command("changechar")]
        public void cmd_changechar(Client client)
        {
            if (!client.HasActiveChar())
            {
                client.SendChatMessage("You are not spawned yet.");
                return;
            }

            var ch = client.GetActiveChar();
            ch?.UpdateAsync();
            client.ResetActiveChar();

            InitCharacterSelection(client);
        }

        [Command("selectchar")]
        public void cmd_selectchar(Client client, int id)
        {
            client.TriggerEvent("selectchar", id);
        }

        [Command("playchar")]
        public void cmd_selectchar(Client client)
        {
            client.TriggerEvent("playchar");
        }

        [Command("ranomizeappearance")]
        public void cmd_randomapp(Client client, int id)
        {
            
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

            InitCharacterSelection(client);
        }

        private void InitCharacterSelection(Client client)
        {
            client.ResetActiveChar();
            client.SendChatMessage("[SERVER]: INIT CHAR SELECTION");
            client.Transparency = 0;
            client.Dimension = (uint)client.Value + 1500;
            client.TriggerEvent(ServerToClient.InitCharSelection);

            var accData = client.GetAccount();
            client.SendChatMessage($"[SERVER]: FETCHING CHARS FOR PLAYER {accData.Username}");
            TaskManager.Run(client, async () =>
            {
                var chars = await Character.FetchAllAsync(accData);
                var charClientList = new List<CharDisplay>();

                foreach (var c in chars)
                {
                    charClientList.Add(new CharDisplay(c.ID, c.CharacterName));
                }
                client.SendChatMessage("[SERVER]: Sending charlist to Client");
                client.TriggerEvent(ServerToClient.RenderCharacterList, JsonConvert.SerializeObject(charClientList), accData.LastSpawnedCharId);
            });
        }

        [RemoteEvent("ApplyCharSelectionAnimation")]
        public void ClientEvent_ApplyCharSelectionAnimation(Client client) => client.PlayAnimation("missbigscore2aleadinout@ig_7_p2@bankman@", "leadout_waiting_loop", 1);

        [RemoteEvent(ClientToServer.SubmitCharacterSelection)]
        public void ClientEvent_SubmitCharacterSelection(Client client, int selectedCharId)
        {
            if(!client.IsLoggedIn()) return;
            if(selectedCharId < 0) return;

            TaskManager.Run(client, async () =>
            {
                var fetchedChar = await Character.ReadAsync(selectedCharId);
                //var cus = fetchedChar.CustomSkin;
                var accData = client.GetAccount();

                if (accData.ID != fetchedChar.CharOwnerID)
                {
                    client.SendChatMessage("That is not your character. Ban/Kick?");
                    return;
                }

                var app = await fetchedChar.GetAppearance();
                if(app == null) throw new Exception($"Character {fetchedChar.CharacterName} ({fetchedChar.ID}) has no appearance data to fetch.");

                app.Apply(client);
                client.Transparency = 255;
            });
        }

        [RemoteEvent(ClientToServer.SubmitSpawnCharacter)]
        public void ClientEvent_SubmitSpawnCharacter(Client client, int selectedCharId)
        {
            if(selectedCharId < 0) return;

            TaskManager.Run(client, async () =>
            {
                var chData = await Character.ReadAsync(selectedCharId);
                var accData = client.GetAccount();
                if (chData.CharOwnerID != accData.ID)
                {
                    client.SendChatMessage("That is not your character. Ban/Kick?");
                    return;
                }

                client.Dimension = 0;
                client.Transparency = 255;
                accData.LastSpawnedCharId = selectedCharId;
                client.SendChatMessage("Teleport to last known position here");
                client.Position = new Vector3(-173.1077, 434.9248, 111.0801);
                client.SetActiveChar(chData);
                client.TriggerEvent(ServerToClient.EndCharSelection);
            });
        }
    }
}
