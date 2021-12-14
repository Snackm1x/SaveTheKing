using Mirror;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Unit : NetworkBehaviour
{
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;
    [SerializeField] private UnitMovement unitMovement = null;

    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;


    [SerializeField]
    private Health health;

    [SerializeField] private Targeter Targeter = null;

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public Targeter GetTargeter()
    {
        return Targeter;
    }

    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
        health.ServerOnDeath += ServerHandleDeath;
    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);
        health.ServerOnDeath -= ServerHandleDeath;
    }

    public override void OnStartClient()
    {
        if(!hasAuthority) { return; }
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) { return; }
        AuthorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        onDeselected?.Invoke();
    }

    [Server]
    private void ServerHandleDeath()
    {
        NetworkServer.Destroy(gameObject);
    }
}
