using Mirror;
using System;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField]
    private Health health;

    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDespawned;

    public override void OnStartServer()
    {
        health.ServerOnDeath += ServerHandleDeath;

        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        health.ServerOnDeath -= ServerHandleDeath;

        ServerOnBaseDespawned?.Invoke(this);
    }

    [Server]
    private void ServerHandleDeath()
    {
        NetworkServer.Destroy(gameObject);
    }
}
