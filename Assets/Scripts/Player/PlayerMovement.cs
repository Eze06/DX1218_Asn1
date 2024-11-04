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
    private InputAction lookAction;
    private InputAction jumpAction;

    [Header("Movement Variables")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Camera Variables")]
    [SerializeField] private float mouseSens = 40f;

    [Header("Jump Variables")]
    [SerializeField] float JumpHeight = 3f;
    [SerializeField] float gravity = -9.81f;


    //Player Movement
    private Vector3 moveDir;

    //Camera
    private Vector2 mouseDelta;

    //Jump
    private Vector3 jumpVelocity;
    

    // Start is called before the first frame update
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Movement"];
        lookAction = playerInput.actions["Look"];
        jumpAction = playerInput.actions["Jump"];

    }

    // Update is called once per frame
    void Update()
    {
        Look();
        Jump();

        HandleGravity();

        Vector2 input = moveAction.ReadValue<Vector2>();

        moveDir = transform.right * input.x + transform.forward * input.y;

        characterController.Move((jumpVelocity + moveDir * moveSpeed) * Time.deltaTime);


    }

    private void Look()
    {
       mouseDelta = lookAction.ReadValue<Vector2>();
       float mouseX = mouseDelta.x * mouseSens * Time.deltaTime;
         
       transform.Rotate(Vector3.up * mouseX);
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
}
