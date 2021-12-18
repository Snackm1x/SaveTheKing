using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUi;
    [SerializeField] private Button startGameButton;

    private void Start()
    {
        STKNetworkManager.ClientOnConnected += HandleClientConnected;
        STKPlayer.AuthorityPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
    }

    private void OnDestroy()
    {
        STKNetworkManager.ClientOnConnected -= HandleClientConnected;
        STKPlayer.AuthorityPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
    }

    private void HandleClientConnected()
    {
        lobbyUi.SetActive(true);
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    public void LeaveLobby()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        } else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }

    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<STKPlayer>().CmdStartGame();
    }
}
