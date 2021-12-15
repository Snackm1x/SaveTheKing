using Mirror;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
    [SerializeField]
    private Health health;
    [SerializeField]
    private int resourcesPerInterval = 10;
    [SerializeField]
    private float resourceGenerationInterval = 2f;

    private float timer;
    private STKPlayer player;

    [ServerCallback]
    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            player.AddResources(resourcesPerInterval);
            timer += resourceGenerationInterval;
        }
    }

    public override void OnStartServer()
    {
        timer = resourceGenerationInterval;
        player = connectionToClient.identity.GetComponent<STKPlayer>();

        health.ServerOnDeath += ServerHandleDeath;
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        health.ServerOnDeath -= ServerHandleDeath;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    private void ServerHandleDeath() 
    {
        NetworkServer.Destroy(gameObject);
    }

    private void ServerHandleGameOver()
    {
        enabled = false;
    }
}
