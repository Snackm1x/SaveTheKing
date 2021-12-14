using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField]
    private int maxHealth = 100;

    [SyncVar(hook=nameof(UpdateHealth))]
    private int currentHealth;

    public event Action ServerOnDeath;
    public event Action<int, int> ClientOnDamageTaken;

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    [Server]
    public void DealDamage(int damageDealt)
    {
        if(currentHealth == 0) { return; }

        currentHealth -= damageDealt;

        if (currentHealth < 0)
            currentHealth = 0;

        if(currentHealth != 0) { return; }

        ServerOnDeath?.Invoke();
    }

    private void UpdateHealth(int oldHealth, int newHealth)
    {
        ClientOnDamageTaken?.Invoke(newHealth, maxHealth);
    }
}
