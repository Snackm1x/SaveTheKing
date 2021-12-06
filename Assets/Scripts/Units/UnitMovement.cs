using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent Agent = null;

    private Camera MainCamera;

    [Command]
    private void CmdMove(Vector3 position)
    {
        if(!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }

        Agent.SetDestination(hit.position);
    }

    public override void OnStartAuthority()
    {
        MainCamera = Camera.main;
    }

    [ClientCallback]

    private void Update()
    {
        if (!hasAuthority) { return; }
        
        if(!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        var ray = MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) { return; }

        CmdMove(hit.point);

    }
}
