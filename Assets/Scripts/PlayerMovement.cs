using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    private bool readyToJump = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space; 


    [Header("Ground check")]
    public float playerHeight;
    public LayerMask groundLayerMask;
    private bool isGrounded; 

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDir;
    private Rigidbody rigibody;

    public MovementState currentState; 
    public enum MovementState
    {
        WALKING,
        AIR
    }

    private void Start()
    {
        rigibody = GetComponent<Rigidbody>();
        rigibody.freezeRotation = true; 
    }

    private void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundLayerMask);
        Debug.Log(isGrounded); 

        GetInput();
        StateHandler(); 
        LimitSpeed(); 

        if (isGrounded)
        {
            rigibody.drag = groundDrag;
        }
        else
        {
            rigibody.drag = 0; 
        }

    }

    private void FixedUpdate()
    {
        MovePlayer(); 
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");


        if(Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
          
            readyToJump = false;
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown); 
        }
    }

    private void StateHandler()
    {
        if (isGrounded)
        {
            currentState = MovementState.WALKING;
        }
        else
        {
            currentState = MovementState.AIR; 
        }
    }

    private void MovePlayer()
    {
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (isGrounded)
        {
            rigibody.AddForce(moveDir * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            rigibody.AddForce(moveDir * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
       
    }

    private void LimitSpeed()
    {
        Vector3 flatVel = new Vector3(rigibody.velocity.x, 0f, rigibody.velocity.z);

     
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rigibody.velocity = new Vector3(limitedVel.x, rigibody.velocity.y, limitedVel.z); 
        }
    }


    private void Jump()
    {
        rigibody.velocity = new Vector3(rigibody.velocity.x, 0f, rigibody.velocity.z);

        rigibody.AddForce(transform.up * jumpForce, ForceMode.Impulse); 
    }

    private void ResetJump()
    {
        readyToJump = true; 
    }


}
