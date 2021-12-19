using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private RectTransform minimapRect;
    [SerializeField] private float mapScale = 50f;
    [SerializeField] private float offSet = -6f;

    private Transform playerCameraTransform;

    private void Start()
    {
        playerCameraTransform = NetworkClient.connection.identity.GetComponent<STKPlayer>().GetCameraTransform();
    }

    private void MoveCamera()
    {
        var mousePosition = Mouse.current.position.ReadValue();

        if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, mousePosition, null, out Vector2 localPoint )) { return; }

        var lerp = new Vector2((localPoint.x - minimapRect.rect.x) / minimapRect.rect.width, (localPoint.y - minimapRect.rect.y) / minimapRect.rect.height);

        var newCameraPosition = new Vector3(Mathf.Lerp(-mapScale, mapScale, lerp.x), playerCameraTransform.position.y, Mathf.Lerp(-mapScale, mapScale, lerp.y));

        playerCameraTransform.position = newCameraPosition + new Vector3(0, 0, offSet);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }
}
