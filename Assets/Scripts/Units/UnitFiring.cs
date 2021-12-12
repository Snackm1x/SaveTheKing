using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField]
    private Targeter targeter = null;
    [SerializeField]
    private GameObject projectilePrefab = null;
    [SerializeField]
    private Transform projectileSpawnPoint = null;
    [SerializeField]
    private float attackRange = 5f;
    [SerializeField]
    private float attackSpeed = 1f;
    [SerializeField]
    private float rotateSpeed = 1f;

    private float timeSinceLastAttack;

    [ServerCallback]
    private void Update()
    {
        var target = targeter.GetTarget();
        if(target == null) { return; }
        if(!CanFireAtTarget()) { return; }

        var targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

        if(Time.time > (1 / attackSpeed) + timeSinceLastAttack)
        {
            var projectileRotation = Quaternion.LookRotation(target.GetTargetLocation().position - projectileSpawnPoint.position);
            var projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);

            NetworkServer.Spawn(projectileInstance, connectionToClient);
            timeSinceLastAttack = Time.time;
        }
    }

    [Server]
    private bool CanFireAtTarget()
    {
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude <= attackRange * attackRange;
    }
}
