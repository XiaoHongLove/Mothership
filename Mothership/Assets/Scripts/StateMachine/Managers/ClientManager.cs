﻿using UnityEngine;
using System.Collections;
using MothershipStateMachine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

public class ClientManager : RoleManager
{
    public ClientNetworkManager NetworkManager { get; private set; }

    // States
    public ClientLobbyState ClientLobbyState { get; private set; }
    public ClientGameSetupState ClientGameSetupState { get; private set; }

    public IAIBase.ETeam Team { get; set; }
    public int TeamOrder { get; set; }

    private PlayerPrefabResourceSO prefabResource = null;
    public PlayerController playerController { get; private set; }

    // Events
    public UnityAction<TeamList, TeamList> OnUpdateTeamRoster { get; set; }

    public override void Init(NetworkManager networkManager)
    {
        NetworkManager = networkManager as ClientNetworkManager;

        ClientLobbyState = new ClientLobbyState(this);
        ClientGameSetupState = new ClientGameSetupState(this);

        activeState = ClientLobbyState;
    }

    public bool SpawnInGame()
    {
        GameObject spawnPoint;
        if(LoadPrefabs() && FindSpawnPoints(out spawnPoint))
        {
            switch(Team)
            {
                case IAIBase.ETeam.TEAM_RED:
                    playerController = Network.Instantiate(prefabResource.RedDrone, spawnPoint.transform.position, spawnPoint.transform.rotation, 0) as PlayerController;
                    return true;
                case IAIBase.ETeam.TEAM_BLUE:
                    playerController = Network.Instantiate(prefabResource.BlueDrone, spawnPoint.transform.position, spawnPoint.transform.rotation, 0) as PlayerController;
                    return true;
                default:
                    return false;
            }
        }
        else
        {
            return false;
        }
    }

    private bool LoadPrefabs()
    {
        prefabResource = Resources.Load<PlayerPrefabResourceSO>("PlayerPrefabResource");
        if(prefabResource != null)
        {
            return true;
        }
        else
        {
            Debug.LogException(new System.NullReferenceException("Prefab Resource not loaded. Cannot create player object!"));
            throw new System.NullReferenceException("Prefab Resource not loaded. Cannot create player object!");
        }
    }

    private bool FindSpawnPoints(out GameObject spawnPoint)
    {
        GameObject group = GameObject.FindGameObjectWithTag("SpawnPoint");
        if(group != null)
        {
            var spawnPoints = group.transform.GetComponentsInChildren<SpawnPoint>().Where(s => s.Team == this.Team).Select(s => s.gameObject).ToList();
            if (spawnPoints.Count > TeamOrder)
            {
                spawnPoint = spawnPoints[TeamOrder];
                return true;
            }
            else
            {
                Debug.LogException(new System.IndexOutOfRangeException("Not enough spawn points"));
                throw new System.IndexOutOfRangeException("Not enough spawn points");
            }

        }
        else
        {
            Debug.LogException(new System.NullReferenceException("Spawn Point Group not found"));
            throw new System.NullReferenceException("Spawn Point Group not found");
        }
    }

}