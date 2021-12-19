using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class STKPlayer : NetworkBehaviour
{
    private List<Unit> units = new List<Unit>();
    private List<Building> buildings = new List<Building>();
    [SerializeField]
    private Building[] possibleBuildings = new Building[0];
    [SyncVar(hook =nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;
    [SerializeField]
    private LayerMask buildingBlockLayer = new LayerMask();
    private float buildingRangeLimit = 25f;
    [SerializeField] private Transform cameraTransform;

    [SyncVar(hook =nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;
    public static event Action<bool> AuthorityPartyOwnerStateUpdated;

    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    private string displayName;


    public event Action<int> ClientOnResourcesUpdated;
    public static event Action ClientOnInfoUpdated;

    public bool GetIsPartyOwner()
    {
        return isPartyOwner;
    }

    public List<Unit> GetUnits()
    {
        return units;
    }

    public List<Building> GetBuildings()
    {
        return buildings;
    }
    public int GetResources()
    {
        return resources;
    }

    public Transform GetCameraTransform()
    {
        return cameraTransform;
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    [Server]
    public void SetDisplayName(string name)
    {
        displayName = name;
    }

    [Server]
    public void AddResources(int resourcesToAdd)
    {
        resources += resourcesToAdd;
    }

    public void RemoveResources(int resourcesToRemove)
    {
        resources -= resourcesToRemove;
    }

    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }

    private void ClientHandleDisplayNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;

        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandleBuildingDespawned;
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 location)
    {
        var buildingToSpawn = possibleBuildings.FirstOrDefault(p => p.GetId() == buildingId);

        if(!buildingToSpawn) { return; }

        if(resources < buildingToSpawn.GetPrice()) { return; }

        var buildingCollider = buildingToSpawn.GetComponent<BoxCollider>();

        if(!CanPlaceBuilding(buildingCollider, location)) { return; }

        var buildingInstance = Instantiate(buildingToSpawn.gameObject, location, buildingToSpawn.transform.rotation);
        NetworkServer.Spawn(buildingInstance, connectionToClient);
        RemoveResources(buildingToSpawn.GetPrice());
    }

    [Command]
    public void CmdStartGame()
    {
        if(!isPartyOwner) { return; }

        ((STKNetworkManager)NetworkManager.singleton).StartGame();
    }

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 location)
    {
        if (Physics.CheckBox(location + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity, buildingBlockLayer))
        {
            return false;
        }

        foreach (var b in buildings)
        {
            if ((location - b.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        };
        return false;
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        units.Add(unit);
    }
    private void ServerHandleUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        units.Remove(unit);
    }
    private void ServerHandleBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        buildings.Add(building);
    }
    private void ServerHandleBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        buildings.Remove(building);
    }

    public override void OnStartAuthority()
    {
        if(NetworkServer.active) { return; }
        Unit.ServerOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += AuthorityHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += AuthorityHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += AuthorityHandleBuildingDespawned;
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();
        if (!isClientOnly) { return; }
        ((STKNetworkManager)NetworkManager.singleton).Players.Remove(this);
        if(!hasAuthority) { return; }
        Unit.ServerOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    public override void OnStartClient()
    {
        Debug.Log("Starting up client!");
        Debug.Log("Am I the host?: " + NetworkServer.active);
        if(NetworkServer.active) { return; }
        Debug.Log("I'm not the host, adding player to list!");
        DontDestroyOnLoad(gameObject);
        ((STKNetworkManager)NetworkManager.singleton).Players.Add(this);
        Debug.Log("Spawned and added player to lobby");
    }

    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
    }

    private void AuthorityHandleUnitSpawned(Unit unit) => units.Add(unit);
    private void AuthorityHandleUnitDespawned(Unit unit) => units.Remove(unit);
    private void AuthorityHandleBuildingSpawned(Building building) => buildings.Add(building);
    private void AuthorityHandleBuildingDespawned(Building building) => buildings.Remove(building);
    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState) 
    {
        if(!hasAuthority) { return; }

        AuthorityPartyOwnerStateUpdated?.Invoke(newState);
    }
}
