using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField]
    private GameObject unitPrefab = null;

    [SerializeField]
    private Transform spawnLocation = null;

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
}
