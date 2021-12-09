using Mirror;
using UnityEngine;

public class Targetable : NetworkBehaviour
{
    [SerializeField]
    private Transform targetLocation;

    public Transform GetTargetLocation() => targetLocation;
}
