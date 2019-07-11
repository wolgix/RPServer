﻿using GTANetworkAPI;
using RPServer.Util;
using Shared;

namespace RPServer.Controllers.EventHandlers
{
    internal class PlayerCommand : Script
    {
        /// <summary>
        /// Event Handler that receives executed commands by relying on the Client.
        /// This cannot block command execution it merely records them.
        /// </summary>
        [RemoteEvent(ClientToServer.SubmitPlayerCommand)]
        public void ClientEvent_OnPlayerCommand(Client client, string cmd)
        {
            // Log the command
            Logger.GetInstance().CommandLog(client.IsLoggedIn() ? client.GetAccount().Username : "UNREGISTERED", cmd);
        }
    }
}
