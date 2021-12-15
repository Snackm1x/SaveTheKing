using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STKPlayer : NetworkBehaviour
{
    private List<Unit> units = new List<Unit>();
    private List<Building> buildings = new List<Building>();

    public List<Unit> GetUnits()
    {
        return units;
    }

    public List<Building> GetBuildings()
    {
        return buildings;
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
