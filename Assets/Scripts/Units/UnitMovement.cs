using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent Agent = null;
    [SerializeField] private Targeter Targeter = null;
    [SerializeField] private float ChaseRange = 10f;

    [ServerCallback]
    private void Update()
    {
        var target = Targeter.GetTarget();

        if(target != null) 
        {
            //Vector3.Distance() does square roots and it's slow, this is faster, but you need to square your range to compensate
            if ((target.transform.position - transform.position).sqrMagnitude > ChaseRange * ChaseRange)
            {
                Agent.SetDestination(target.transform.position);
            }
            else if (Agent.hasPath)
            {
                Agent.ResetPath();
            }
            return; 
        }
        if(!Agent.hasPath) { return; }
        if(Agent.remainingDistance > Agent.stoppingDistance) { return; }
        Agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        Targeter.ClearTarget();
        if(!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }

        Agent.SetDestination(hit.position);
    }

    [Command]
    public void ChaseUnit()
    {

    }


}
