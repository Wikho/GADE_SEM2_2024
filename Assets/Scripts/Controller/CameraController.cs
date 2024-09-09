using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    #region Variables
    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 5f;
    [SerializeField] private float runMultiplier = 2f;
    [SerializeField] private float rotationSpeed = 100f;

    [Header("Camera Settings")]
    [SerializeField] private float verticalRotationLimit = 80f; //Limit for looking up and down

    //private
    private Vector2 movementInput;
    private Vector2 lookInput;
    private bool isRunning;
    private bool isRotating;
    private float verticalRotation = 0f;

    private NewInputSystem playerInput;
    private InputAction moveAction;
    private InputAction runAction;
    private InputAction lookAction;
    private InputAction rotateCameraAction;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        playerInput = new NewInputSystem();

        var cameraMovementActionMap = playerInput.CameraMovement;

        moveAction = cameraMovementActionMap.Move;
        runAction = cameraMovementActionMap.Run;
        lookAction = cameraMovementActionMap.Look;
        rotateCameraAction = cameraMovementActionMap.RotateCamera;

        moveAction.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => movementInput = Vector2.zero;

        runAction.performed += ctx => isRunning = ctx.ReadValueAsButton();
        runAction.canceled += ctx => isRunning = false;

        lookAction.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        lookAction.canceled += ctx => lookInput = Vector2.zero;

        rotateCameraAction.performed += ctx => isRotating = ctx.ReadValueAsButton();
        rotateCameraAction.canceled += ctx => isRotating = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    #endregion

    #region Enable/Disbale
    private void OnEnable()
    {
        moveAction.Enable();
        runAction.Enable();
        lookAction.Enable();
        rotateCameraAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        runAction.Disable();
        lookAction.Disable();
        rotateCameraAction.Disable();
    }

    #endregion

    #region Functions

    private void HandleMovement()
    {
        float speed = isRunning ? normalSpeed * runMultiplier : normalSpeed;
        Vector3 direction = new Vector3(movementInput.x, 0, movementInput.y).normalized;
        transform.Translate(direction * speed * Time.deltaTime, Space.Self);
    }

    private void HandleRotation()
    {
        if (isRotating)
        {
            // Horizontal rotation (left-right)
            float horizontalRotation = lookInput.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, horizontalRotation, Space.World);

            // Vertical rotation (up-down)
            verticalRotation -= lookInput.y * rotationSpeed * Time.deltaTime;
            verticalRotation = Mathf.Clamp(verticalRotation, -verticalRotationLimit, verticalRotationLimit);

            // Apply vertical rotation to the camera
            Camera.main.transform.localEulerAngles = new Vector3(verticalRotation, Camera.main.transform.localEulerAngles.y, 0);
        }
    }

    #endregion
}
