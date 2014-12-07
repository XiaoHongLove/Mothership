﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MothershipStateMachine
{
    public class ServerLobbyState : IState
    {
        private ServerManager manager = null;

        public ServerLobbyState(ServerManager manager)
        {
            this.manager = manager;
        }

        public void OnGameMessage(GameMessage message)
        {
            RegisterClient registerClient = message as RegisterClient;
            if(registerClient != null)
            {
                if(manager.RegisterClient(registerClient))
                {
                    IEnumerable<ClientDataOnServer> redTeam = manager.GetTeam(IAIBase.ETeam.TEAM_RED);
                    TeamList redList = new TeamList(IAIBase.ETeam.TEAM_RED, redTeam.Select(c => c.Profile.DisplayName).ToArray());

                    IEnumerable<ClientDataOnServer> blueTeam = manager.GetTeam(IAIBase.ETeam.TEAM_BLUE);
                    TeamList blueList = new TeamList(IAIBase.ETeam.TEAM_BLUE, blueTeam.Select(c => c.Profile.DisplayName).ToArray());

                    manager.networkManager.SendTeamSetupUpdate(redList, blueList);
                    return;
                }
            }

            ClientReadyToPlay clientReady = message as ClientReadyToPlay;
            if(clientReady != null)
            {
                manager.RegisteredClients.First(c => c.NetworkPlayer == clientReady.Player).ReadyToPlay = true;
                if(manager.RegisteredClients.Count() >= manager.MinPlayersInGame &&  manager.RegisteredClients.All(c => c.ReadyToPlay == true))
                {
                    manager.networkManager.StartMission();
                }
                return;
            }
        }

        public void OnStateMessage(StateMessage message)
        {

        }        
    }
}