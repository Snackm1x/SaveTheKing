using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField]
    private Rigidbody rb = null;
    [SerializeField]
    private float launchForce = 10f;
    [SerializeField]
    private float lifespan = 5f;
    [SerializeField]
    private int damageToDeal = 20;

    private void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if(networkIdentity.connectionToClient == connectionToClient) { return; }
        }

        if(other.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage(damageToDeal);
        }

        DestroySelf();
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), lifespan);
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}
