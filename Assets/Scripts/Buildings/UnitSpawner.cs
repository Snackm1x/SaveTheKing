using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject unitPrefab;

    [SerializeField]
    private Transform spawnLocation;

    [SerializeField]
    private Health health;

    public override void OnStartServer()
    {
        health.ServerOnDeath += ServerHandleDeath;
    }

    public override void OnStopServer()
    {
        health.ServerOnDeath -= ServerHandleDeath;
    }

    [Command]
    private void CmdSpawnUnit()
    {
        var unitInstance = Instantiate(unitPrefab, spawnLocation.position, spawnLocation.rotation);
        NetworkServer.Spawn(unitInstance, connectionToClient);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        if(!hasAuthority) { return; }

        CmdSpawnUnit();
    }

    [Server]
    private void ServerHandleDeath()
    {
        NetworkServer.Destroy(gameObject);
    }
}
