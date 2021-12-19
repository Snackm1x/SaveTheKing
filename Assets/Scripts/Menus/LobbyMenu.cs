using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUi;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];

    private void Start()
    {
        STKNetworkManager.ClientOnConnected += HandleClientConnected;
        STKPlayer.AuthorityPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        STKPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    private void OnDestroy()
    {
        STKNetworkManager.ClientOnConnected -= HandleClientConnected;
        STKPlayer.AuthorityPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        STKPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }

    private void HandleClientConnected()
    {
        Debug.Log("Client connected turning on lobby UI");
        Debug.Log("Lobby UI: " + lobbyUi);
        lobbyUi.SetActive(true);
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    private void ClientHandleInfoUpdated()
    {
        var players = ((STKNetworkManager)NetworkManager.singleton).Players;

        var index = 0;
        players.ForEach(p =>
        {
            playerNameTexts[index].text = p.GetDisplayName();
            index++;
        });

        for(var i = players.Count; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = "Waiting For Player...";
        }

        startGameButton.interactable = players.Count >= 2;
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
