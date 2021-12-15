using Mirror;
using System;
using System.Collections;
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

    public event Action<int> ClientOnResourcesUpdated;

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

    [Server]
    public void AddResources(int resourcesToAdd)
    {
        resources += resourcesToAdd;
    }

    private void ClientHandleResourcesUpdated(int oldResources, int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }


    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandleBuildingDespawned;
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

        var buildingInstance = Instantiate(buildingToSpawn.gameObject, location, buildingToSpawn.transform.rotation);
        NetworkServer.Spawn(buildingInstance, connectionToClient);
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
        if (!isClientOnly || !hasAuthority) { return; }
        Unit.ServerOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= AuthorityHandleUnitDespawned;
        Building.ServerOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.ServerOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }

    private void AuthorityHandleUnitSpawned(Unit unit) => units.Add(unit);
    private void AuthorityHandleUnitDespawned(Unit unit) => units.Remove(unit);
    private void AuthorityHandleBuildingSpawned(Building building) => buildings.Add(building);
    private void AuthorityHandleBuildingDespawned(Building building) => buildings.Remove(building);
}
