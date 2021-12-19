using Mirror;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text resourcesText;
    [SerializeField]
    private STKPlayer player;

    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<STKPlayer>();
        ClientHandleResourcesUpdated(player.GetResources());
        player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
    }

    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(int resources)
    {
        resourcesText.text = $"Resources: {resources}";
    }
}
