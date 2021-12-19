using Mirror;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class STKNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitBasePrefab;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab;

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    private bool isGameInProgress = false;
    public List<STKPlayer> Players { get; } = new List<STKPlayer>();

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        Debug.Log("Server on client connect called!");
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ClientOnDisconnected?.Invoke();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        var player = conn.identity.GetComponent<STKPlayer>();

        Players.Add(player);
        var steamName = SteamFriends.GetPersonaName();
        Debug.Log("Player added to server named: " + steamName);

        player.SetDisplayName(steamName);

        player.SetPartyOwner(Players.Count == 1);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            var gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

            Players.ForEach(p =>
            {
                var baseInstance = Instantiate(unitBasePrefab, GetStartPosition().position, Quaternion.identity);
                NetworkServer.Spawn(baseInstance, p.connectionToClient);
            });
        }
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if(!isGameInProgress) { return; }
        Debug.Log("Server OnServerConnect! " + conn);
        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        var player = conn.identity.GetComponent<STKPlayer>();

        Players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Players.Clear();

        isGameInProgress = false;
    }

    public void StartGame()
    {
        if(Players.Count < 2) { return; }

        isGameInProgress = true;

        ServerChangeScene("Scene_Map_01");
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }
}
