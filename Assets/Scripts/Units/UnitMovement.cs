using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent Agent = null;


    [ServerCallback]
    private void Update()
    {
        if(!Agent.hasPath) { return; }
        if(Agent.remainingDistance > Agent.stoppingDistance) { return; }
        Agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        if(!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }

        Agent.SetDestination(hit.position);
    }


}
