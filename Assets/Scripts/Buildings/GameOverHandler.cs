using Mirror;
using System;
using System.Collections.Generic;

public class GameOverHandler : NetworkBehaviour
{
    public static event Action ServerOnGameOver;
    public static event Action<string> ClientOnGameOver;

    private List<UnitBase> bases = new List<UnitBase>();
    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }

    [Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        bases.Add(unitBase);
    }

    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);

        if(bases.Count != 1) { return; }

        var winner = bases[0].connectionToClient.connectionId;

        RpcGameOver($"Player {winner}");
        ServerOnGameOver?.Invoke();
    }

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }
}
