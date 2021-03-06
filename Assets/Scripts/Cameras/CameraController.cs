using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private float Speed = 20f;
    [SerializeField] private float screenBorderThickness = 10f;
    [SerializeField] private Vector2 screenXLimits = Vector2.zero;
    [SerializeField] private Vector2 screenZLimits = Vector2.zero;

    private STKControls controls;
    private Vector2 previousInput;

    public override void OnStartAuthority()
    {
        playerCameraTransform.gameObject.SetActive(true);

        controls = new STKControls();

        controls.Player.MoveCamera.performed += SetPreviousInput;
        controls.Player.MoveCamera.canceled += SetPreviousInput;

        controls.Enable();
    }

    [ClientCallback]
    private void Update()
    {
        if(!hasAuthority || !Application.isFocused) { return; }

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        var position = playerCameraTransform.position;

        if(previousInput == Vector2.zero)
        {
            var cursorMovement = Vector3.zero;
            var cursorPosition = Mouse.current.position.ReadValue();

            if(cursorPosition.y >= Screen.height - screenBorderThickness)
            {
                cursorMovement.z += 1;
            } 
            else if(cursorPosition.y <= screenBorderThickness) 
            {
                cursorMovement.z -= 1;
            }
            if (cursorPosition.x >= Screen.width - screenBorderThickness)
            {
                cursorMovement.x += 1;
            }
            else if (cursorPosition.x <= screenBorderThickness)
            {
                cursorMovement.x -= 1;
            }

            position += cursorMovement.normalized * Speed * Time.deltaTime;
        }
        else
        {
            position += new Vector3(previousInput.x, 0, previousInput.y) * Speed * Time.deltaTime;
        }

        position.x = Mathf.Clamp(position.x, screenXLimits.x, screenXLimits.y);
        position.z = Mathf.Clamp(position.z, screenZLimits.x, screenZLimits.y);

        playerCameraTransform.position = position;
    }

    private void SetPreviousInput(InputAction.CallbackContext ctx)
    {
        previousInput = ctx.ReadValue<Vector2>();
    }
}
