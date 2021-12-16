using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Unit unitPrefab;

    [SerializeField]
    private Transform spawnLocation;

    [SerializeField]
    private Health health;
    [SerializeField]
    private TMP_Text remainingUnitsText;
    [SerializeField]
    private Image unitProgressImage;
    [SerializeField]
    private int maxUnitQueue = 5;
    [SerializeField]
    private float spawnMoveRange = 7f;
    [SerializeField]
    private float unitSpawnDuration = 5f;

    [SyncVar(hook =nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;
    [SyncVar]
    private float unitTimer;

    private float progressImageVelocity;

    private void Update()
    {
        if(isServer)
        {
            ProduceUnits();
        }

        if (isClient)
        {
            UpdateTimerDisplay();
        }
    }

    public override void OnStartServer()
    {
        health.ServerOnDeath += ServerHandleDeath;
    }

    public override void OnStopServer()
    {
        health.ServerOnDeath -= ServerHandleDeath;
    }

    private void ClientHandleQueuedUnitsUpdated(int oldQueuedUnits, int newQueuedUnits)
    {
        remainingUnitsText.text = newQueuedUnits.ToString();
    }

    [Server]
    private void ProduceUnits()
    {
        if(queuedUnits == 0) { return; }
        unitTimer += Time.deltaTime;
        if(unitTimer < unitSpawnDuration) { return; }
        var unitInstance = Instantiate(unitPrefab, spawnLocation.position, spawnLocation.rotation);
        NetworkServer.Spawn(unitInstance.gameObject, connectionToClient);

        var spawnOffset = Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = spawnLocation.position.y;

        var unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(spawnLocation.position + spawnOffset);

        queuedUnits--;
        unitTimer = 0f;
    }



    [Command]
    private void CmdSpawnUnit()
    {
        if(queuedUnits == maxUnitQueue) { return; }

        var player = connectionToClient.identity.GetComponent<STKPlayer>();

        if(player.GetResources() < unitPrefab.GetResourceCost()) { return; }

        queuedUnits++;

        player.RemoveResources(unitPrefab.GetResourceCost());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }

        if(!hasAuthority) { return; }

        CmdSpawnUnit();
    }

    [Server]
    private void ServerHandleDeath()
    {
        NetworkServer.Destroy(gameObject);
    }

    private void UpdateTimerDisplay()
    {
        var progress = unitTimer / unitSpawnDuration;
        if(progress < unitProgressImage.fillAmount)
        {
            unitProgressImage.fillAmount = progress;
        } 
        else
        {
            unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount, progress, ref progressImageVelocity, 0.1f);
        }
    }
}
