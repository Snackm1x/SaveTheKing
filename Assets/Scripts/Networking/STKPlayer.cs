using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class STKPlayer : NetworkBehaviour
{
    [SerializeField]
    private List<Unit> units = new List<Unit>();

    public List<Unit> GetPlayerUnits()
    {
        return units;
    }

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
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

    public override void OnStartAuthority()
    {
        if(NetworkServer.active) { return; }
        Unit.ServerOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += AuthorityHandleUnitDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly || !hasAuthority) { return; }
        Unit.ServerOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }

    private void AuthorityHandleUnitSpawned(Unit unit) => units.Add(unit);
    private void AuthorityHandleUnitDespawned(Unit unit) => units.Remove(unit);
}
