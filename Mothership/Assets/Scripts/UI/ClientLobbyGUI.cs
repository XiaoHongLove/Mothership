﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using Pathfinding.Serialization.JsonFx;
using System.Collections.Generic;
using MothershipStateMachine;

namespace MothershipUI
{
    public class ClientLobbyGUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject content = null;

        [SerializeField]
        private WaitScreenGUI waitScreen = null;

        [SerializeField]
        private Text title = null;
        [SerializeField]
        private List<Text> redTeamList = null;
        [SerializeField]
        private List<Text> blueTeamList = null;

        [SerializeField]
        private Button disconnectButton = null;
        [SerializeField]
        private Button readyButton = null;

        private ClientManager clientManager = null;

        private void Start()
        {
            readyButton.onClick.AddListener(() => { clientManager.SendGameMessage(new ClientReadyToPlay()); readyButton.interactable = false; });
        }

        public void EnableScreen()
        {
            clientManager = RoleManager.roleManager as ClientManager;

            title.text = clientManager.NetworkManager.ServerHostData.gameName + " - Lobby";

            waitScreen.Enable("Waiting For Server");
            clientManager.OnUpdateTeamRoster += (team1, team2) => { waitScreen.Disable(); UpdateTeamRoster(team1, team2); };
            clientManager.NetworkManager.RegisterOnServer();
            content.SetActive(true);
        }

        public void DisableScreen()
        {
            content.SetActive(false);
        }

        private void UpdateTeamRoster(TeamList team1, TeamList team2)
        {
            UpdateTeam(team1);
            UpdateTeam(team2);
        }

        private void UpdateTeam(TeamList team)
        {
            int i = 0;
            switch (team.TeamColour)
            {
                case IAIBase.ETeam.TEAM_BLUE:
                    foreach (string player in team.TeamDisplayNames)
                    {
                        blueTeamList[i].text = player;
                        i++;
                    }

                    // Continue looping through the display and fill remaining lisings with placeholder text
                    for (; i < blueTeamList.Count; i++)
                    {
                        blueTeamList[i].text = "[AI]";
                    }

                    break;

                case IAIBase.ETeam.TEAM_RED:
                    foreach (string player in team.TeamDisplayNames)
                    {
                        redTeamList[i].text = player;
                        i++;
                    }

                    // Continue looping through the display and fill remaining lisings with placeholder text
                    for (; i < redTeamList.Count; i++)
                    {
                        redTeamList[i].text = "[AI]";
                    }

                    break;
            }
        }
    }
}