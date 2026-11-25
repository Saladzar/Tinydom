using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private InputSystem_Actions inputActions;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 0.5f;

    private Vector3 direction { get { return inputActions.Player.MoveCamera.ReadValue<Vector3>().normalized; } }
    private bool isRotating { get { return inputActions.Player.RotateCamera.inProgress; } }

    void Awake()
    {
        Init();
    }

    void Update()
    {
        if (MenuManager.Instance.IsPaused())
            return;

        CameraMovement();
        CameraRotation();

        if (InterfaceManager.Instance.interfaceOpen && (direction != Vector3.zero || isRotating == true))
            InterfaceManager.Instance.ExitInterface();
    }

    private void CameraRotation()
    {
        if (isRotating)
        {
            Vector2 mouseDelta = inputActions.Player.MouseDirection.ReadValue<Vector2>();
            mainCamera.transform.Rotate(Vector3.up, mouseDelta.x * rotateSpeed, Space.World);
            mainCamera.transform.Rotate(Vector3.right, -mouseDelta.y * rotateSpeed, Space.Self);
        }

        Cursor.lockState = isRotating ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !isRotating;
    }

    private void CameraMovement()
    {
        Vector3 forward = mainCamera.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = mainCamera.transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 movement = right * direction.x + Vector3.up * direction.y + forward * direction.z;
        mainCamera.transform.position += movement * moveSpeed * Time.deltaTime;
    }

    private void Init()
    {
        inputActions = new InputSystem_Actions();
        inputActions.Player.Enable();
    }
}
