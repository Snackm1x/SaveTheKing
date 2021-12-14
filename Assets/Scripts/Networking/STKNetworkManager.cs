using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class STKNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitSpawnerPrefab;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab;
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        var unitSpanwerInstance = Instantiate(unitSpawnerPrefab, conn.identity.transform.position, conn.identity.transform.rotation);
        NetworkServer.Spawn(unitSpanwerInstance, conn);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            var gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
        }
    }
}
