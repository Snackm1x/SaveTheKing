using Mirror;
using UnityEngine;
using Steamworks;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel;

    [SerializeField] private bool useSteam = false;

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private CSteamID hostLobbyId;


    private void Start()
    {
        if(!useSteam) { return; }

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        landingPagePanel.SetActive(false);

        if (useSteam)
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
            return;
        }

        NetworkManager.singleton.StartHost();
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if(callback.m_eResult != EResult.k_EResultOK)
        {
            landingPagePanel.SetActive(true);
            return;
        }

        NetworkManager.singleton.StartHost();

        Debug.Log("Lobby Created with: " + new CSteamID(callback.m_ulSteamIDLobby));

        hostLobbyId = new CSteamID(callback.m_ulSteamIDLobby);
        SteamMatchmaking.SetLobbyData(hostLobbyId, "HostAddress", SteamUser.GetSteamID().ToString());
    }

    private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        Debug.Log("Joining Game ID: " + callback.m_steamIDLobby);
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if(NetworkServer.active) { return; }

        Debug.Log("Cached lobby id: " + hostLobbyId);

        var lobby = new CSteamID(callback.m_ulSteamIDLobby);

        var hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "HostAddress");

        Debug.Log("Entered lobby: " + new CSteamID(callback.m_ulSteamIDLobby));
        var numberOfPlayers = SteamMatchmaking.GetNumLobbyMembers(new CSteamID(callback.m_ulSteamIDLobby));
        Debug.Log("Number of connected Players: " + numberOfPlayers);
        Debug.Log("Host address of lobby: " + hostAddress);
        NetworkManager.singleton.networkAddress = hostAddress;
        Debug.Log("Network start client");
        NetworkManager.singleton.StartClient();

        Debug.Log("Landing page panel: " + landingPagePanel);

        if(landingPagePanel != null)
        {
            landingPagePanel.SetActive(false);
        }
    }

    public void OnInviteUser()
    {
        Debug.Log("Opening invite dialog with lobby id: " + hostLobbyId);
        SteamFriends.ActivateGameOverlayInviteDialog(hostLobbyId);
    }
}
