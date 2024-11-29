using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class PlayerController : MonoBehaviour
{
    
    private PlayerInput playerInput;
    [HideInInspector] public CharacterController characterController;
    private CameraAnimation cameraAnimation;

    //Input Actions
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction lookAction;
    [HideInInspector] public InputAction sprintAction;
    private InputAction crouchAction;
    [HideInInspector] public InputAction shootAction;
    [HideInInspector] public InputAction switchFireModeAction;
    [HideInInspector] public InputAction ADSAction;
    [HideInInspector] public InputAction DropAction;
    [HideInInspector] public InputAction SelectPrimaryAction;
    [HideInInspector] public InputAction SelectSecondaryAction;
    [HideInInspector] public InputAction InteractAction;
    [HideInInspector] public InputAction ReloadAction;


    [Header("Movement Variables")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float walkSpeedMult = 1f;
    [SerializeField] private float sprintSpeedMult = 2f;
    [SerializeField] private float crouchSpeedMult = 0.5f;
    [SerializeField] private float moveSpeedTransition;

    [Header("Jump Variables")]
    [SerializeField] float JumpHeight = 3f;
    [SerializeField] float gravity = -9.81f;


    [Header("Camera Variables")]
    [SerializeField] private float mouseSens = 40f;
    [SerializeField] private float bobFrequency = 1f;
    [SerializeField] private float bobAmplitude = 3f;
    [SerializeField] private float sprintFOVMultiplier = 0.8f;

    [Header("Crouch Variables")]
    [SerializeField] private float crouchHeight = 0.5f;
    private float normalHeight;

    //Camera
    [HideInInspector] public Vector2 mouseDelta;
    private float cameraPitch;




    //Player Movement
    [HideInInspector] public Vector3 moveDir;

    //Jump
    [HideInInspector] public Vector3 jumpVelocity;

    //Sprint
    [HideInInspector] public bool isSprinting;
    private float speed;
    [HideInInspector] public float currentSpeedMultiplier;


    // Start is called before the first frame update
    void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraAnimation = GameObject.Find("CameraAnimator").GetComponent<CameraAnimation>();

        moveAction = playerInput.actions["Movement"];
        jumpAction = playerInput.actions["Jump"];
        lookAction = playerInput.actions["Look"];
        sprintAction = playerInput.actions["Sprint"];
        crouchAction = playerInput.actions["Crouch"];
        shootAction = playerInput.actions["Shoot"];
        switchFireModeAction = playerInput.actions["SwitchFireMode"];
        ADSAction = playerInput.actions["ADS"];
        DropAction = playerInput.actions["Drop"];
        SelectPrimaryAction = playerInput.actions["SelectPrimary"];
        SelectSecondaryAction = playerInput.actions["SelectSecondary"];
        InteractAction = playerInput.actions["Interact"];
        ReloadAction = playerInput.actions["Reload"];

        normalHeight = characterController.height;


    }

    // Update is called once per frame
    void Update()
    {
        LookX();
        Jump();
        HandleGravity();
        HandleSprint();
        HandleCrouch();   


        Vector2 input = moveAction.ReadValue<Vector2>();

        moveDir = transform.right * input.x + transform.forward * input.y;

        speed = Mathf.Lerp(speed, moveSpeed * currentSpeedMultiplier, moveSpeedTransition * Time.deltaTime);

        characterController.Move((jumpVelocity + moveDir * speed) * Time.deltaTime);


    }

    private void LateUpdate()
    {
        if (characterController.isGrounded == false)
            return;

        bool isMoving = moveDir.magnitude > 0;
        
        if(isMoving)
        {
            cameraAnimation.HeadBob(currentSpeedMultiplier);
        }

        HandleCameraPitch();
    }

    private void LookX()
    {
        mouseDelta = lookAction.ReadValue<Vector2>();
        float mouseX = mouseDelta.x * mouseSens * Time.deltaTime;

        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleCameraPitch()
    {
        float mouseY = mouseDelta.y * mouseSens * Time.deltaTime;

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
        Camera.main.transform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);
    }

    private void HandleGravity()
    {

        if (characterController.isGrounded && jumpVelocity.y < 0)
        {
            jumpVelocity.y = -2f;
        }

        jumpVelocity.y += gravity * Time.deltaTime;
    }

    private void Jump()
    {
        if(jumpAction.IsPressed() && characterController.isGrounded)
        {
            jumpVelocity.y = Mathf.Sqrt(JumpHeight * -2f * gravity);
        }
    }


    private void HandleSprint()
    {
        if (crouchAction.IsPressed()) return; // Cannot sprint if player is crouched

        if (sprintAction.IsPressed())
        {
            currentSpeedMultiplier = sprintSpeedMult;
            isSprinting = true;
            cameraAnimation.zoomSprint = true;
        }
        else
        {
            currentSpeedMultiplier = walkSpeedMult;
            cameraAnimation.zoomSprint = false;
            isSprinting = false;
        }

    }

    private void HandleCrouch()
    {
        if(crouchAction.IsPressed())
        {
            currentSpeedMultiplier = crouchSpeedMult;
            characterController.height = crouchHeight;
            characterController.center = new Vector3(0, crouchHeight / 2, 0);
        }
        else
        {
            characterController.height = normalHeight;
            characterController.center = new Vector3(0, 0, 0);
        }
    }
    

}
