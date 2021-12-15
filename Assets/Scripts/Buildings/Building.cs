using Mirror;
using System;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField]
    private GameObject buildingPreview;
    [SerializeField]
    private Sprite icon;
    [SerializeField]
    private int price = 100;
    [SerializeField] 
    private int buildingId = -1;

    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;

    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;

    public Sprite GetIcon()
    {
        return icon;
    }

    public int GetPrice()
    {
        return price;
    }

    public int GetId()
    {
        return buildingId;
    }

    public GameObject GetBuildingPreview()
    {
        return buildingPreview;
    }

    public override void OnStartServer()
    {
        ServerOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnBuildingDespawned?.Invoke(this);
    }

    public override void OnStartClient()
    {
        if (!hasAuthority) { return; }
        AuthorityOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if (!hasAuthority) { return; }
        AuthorityOnBuildingDespawned?.Invoke(this);
    }
}
