using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField]
    private UnitSelectionHandler unitSelectionHandler = null;

    [SerializeField]
    private LayerMask layerMask = new LayerMask();

    private Camera MainCamera;

    private void Start()
    {
        MainCamera = Camera.main;
    }

    private void Update()
    {
        if(!Mouse.current.rightButton.wasPressedThisFrame) { return; }

        var ray = MainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }

        if(hit.collider.TryGetComponent(out Targetable target))
        {
            if (target.hasAuthority)
            {
                TryMove(hit.point);
                return;
            }

            TryAttack(target);
            return;
        }

        TryMove(hit.point);
    }

    private void TryMove(Vector3 point)
    {
        unitSelectionHandler.SelectedUnits.ForEach(unit => unit.GetUnitMovement().CmdMove(point));
    }

    private void TryAttack(Targetable target)
    {
        unitSelectionHandler.SelectedUnits.ForEach(unit => unit.GetTargeter().CmdSetTarget(target.gameObject));
    }
}
