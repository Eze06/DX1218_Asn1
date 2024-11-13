using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput playerInput;
    private CharacterController characterController;

    //Input Actions
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction lookAction;
    private InputAction sprintAction;
    private InputAction crouchAction;

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
    private Vector2 mouseDelta;
    private float cameraPitch;
    private float sprintFOV;
    private float normalFOV;
    private float currentFOV;

    //Camera Bob
    private float originalCameraY;
    private float timer;
    private float bobbingOffset;


    //Player Movement
    private Vector3 moveDir;

    //Jump
    private Vector3 jumpVelocity;

    //Sprint
    private bool isSprinting;
    private float speed;
    private float currentSpeedMultiplier;


    // Start is called before the first frame update
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Movement"];
        jumpAction = playerInput.actions["Jump"];
        lookAction = playerInput.actions["Look"];
        sprintAction = playerInput.actions["Sprint"];
        crouchAction = playerInput.actions["Crouch"];

        normalHeight = characterController.height;

        originalCameraY = Camera.main.transform.localPosition.y;
        timer = 0f;
        bobbingOffset = 0f;

        currentFOV = normalFOV = Camera.main.fieldOfView;
        sprintFOV = normalFOV * sprintFOVMultiplier;
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
        HandleCameraBob();
        HandleCameraPitch();
        HandleCameraFOV();
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

    private void HandleCameraBob()
    {
        if (!characterController.isGrounded) return;

        Transform cameraTransform = Camera.main.transform;

        bool isMoving = moveDir.magnitude > 0;

        if(isMoving)
        {
            timer += Time.deltaTime * bobFrequency * currentSpeedMultiplier;
                
            bobbingOffset = Mathf.Sin(timer) * bobAmplitude;
        }
        else
        {
            bobbingOffset = Mathf.Lerp(bobbingOffset, 0, Time.deltaTime);
        }

        cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x, originalCameraY + bobbingOffset, cameraTransform.localPosition.z);
    }

    private void HandleSprint()
    {
        if (crouchAction.IsPressed()) return; // Cannot sprint if player is crouched

        if (sprintAction.IsPressed())
        {
            currentSpeedMultiplier = sprintSpeedMult;
            isSprinting = true;
        }
        else
        {
            currentSpeedMultiplier = walkSpeedMult;
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
    
    private void HandleCameraFOV()
    {
        if(isSprinting)
        {
            currentFOV = Mathf.Lerp(currentFOV, sprintFOV, moveSpeedTransition * Time.deltaTime);
        }
        else
        {
            currentFOV = Mathf.Lerp(currentFOV, normalFOV, moveSpeedTransition * Time.deltaTime);
        }

        Camera.main.fieldOfView = currentFOV;
    }
}
