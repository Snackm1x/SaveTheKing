using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField]
    private LayerMask SelectionLayerMask = new LayerMask();
    [SerializeField]
    private RectTransform UnitSelectionArea = null;
    private STKPlayer stkPlayer;

    private Vector2 dragSelectionStartLocation;

    private Camera MainCamera;

    public List<Unit> SelectedUnits { get; } = new List<Unit>();

    private void Start()
    {
        MainCamera = Camera.main;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void Update()
    {
        if(stkPlayer == null)
        {
            stkPlayer = NetworkClient.connection.identity.GetComponent<STKPlayer>();
        }

        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }
    }

    private void ClearSelectionArea()
    {

        UnitSelectionArea.gameObject.SetActive(false);

        if(UnitSelectionArea.sizeDelta.magnitude == 0) // clicked and didn't drag
        {
            var ray = MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, SelectionLayerMask)) { return; }

            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) { return; }

            if (!unit.hasAuthority) { return; }

            SelectedUnits.Add(unit);

            SelectedUnits.ForEach(selectedUnit => selectedUnit.Select());

            return;
        }

        var min = UnitSelectionArea.anchoredPosition - (UnitSelectionArea.sizeDelta / 2);
        var max = UnitSelectionArea.anchoredPosition + (UnitSelectionArea.sizeDelta / 2);

        stkPlayer.GetUnits().ForEach(unit =>
        {
            if(SelectedUnits.Contains(unit)) { return; }
            var screenPosition = MainCamera.WorldToScreenPoint(unit.transform.position);
            
            if(screenPosition.x > min.x && screenPosition.x < max.x && screenPosition.y > min.y && screenPosition.y < max.y)
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        });
    }

    private void StartSelectionArea()
    {
        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            SelectedUnits.ForEach(selectedUnit => selectedUnit.Deselect());
            SelectedUnits.Clear();
        }

        UnitSelectionArea.gameObject.SetActive(true);

        dragSelectionStartLocation = Mouse.current.position.ReadValue();

        UpdateSelectionArea();
    }
    private void UpdateSelectionArea()
    {
        var mousePosition = Mouse.current.position.ReadValue();

        var areaWidth = mousePosition.x - dragSelectionStartLocation.x;
        var areaHeight = mousePosition.y - dragSelectionStartLocation.y;

        UnitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        UnitSelectionArea.anchoredPosition = dragSelectionStartLocation + new Vector2(areaWidth / 2, areaHeight / 2);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        if (SelectedUnits.Contains(unit)) SelectedUnits.Remove(unit);
    }

    private void ClientHandleGameOver(string winner)
    {
        enabled = false;
    }
}
