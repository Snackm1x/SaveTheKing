using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class STKNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitBasePrefab;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab;
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        var unitBaseInstance = Instantiate(unitBasePrefab, conn.identity.transform.position, conn.identity.transform.rotation);
        NetworkServer.Spawn(unitBaseInstance, conn);
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
