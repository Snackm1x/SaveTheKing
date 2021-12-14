using Mirror;
using System;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField]
    private Health health;

    public static event Action<UnitBase> ServerOnBaseSpawned;
    public static event Action<UnitBase> ServerOnBaseDespawned;
    public static event Action<int> ServerOnPlayerDied;

    public override void OnStartServer()
    {
        ServerOnBaseSpawned?.Invoke(this);
        health.ServerOnDeath += ServerHandleDeath;
    }

    public override void OnStopServer()
    {
        ServerOnBaseDespawned?.Invoke(this);
        health.ServerOnDeath -= ServerHandleDeath;
    }

    [Server]
    private void ServerHandleDeath()
    {
        ServerOnPlayerDied?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);
    }
}
