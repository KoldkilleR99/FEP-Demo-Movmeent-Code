using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementNoAim : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float groundSpeed;
    public float grappleSpeed;
    public float groundDrag;
    public float jumpForce;
    public float doubleJumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public bool readyToJump;
    public bool hasJumped1;
    public bool freeze;
    public bool doubleJumped;
    public bool activeGrapple;
    public float swingSpeed;
    public bool swinging;

    [Header("Ground Check")]
    public float playerHeight;
    public bool isGrounded;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        doubleJumped = false;
    }

    public void Update()
    {
        if (isGrounded && !activeGrapple)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        if (freeze)
        {
            rb.velocity = Vector3.zero;
        }

        if (swinging)
        {
            moveSpeed = swingSpeed;

        }

        if (!swinging)
        {
            moveSpeed = groundSpeed;

        }

        MyInput();
        SpeedControl();
        DoubleJump();
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(jumpKey) && readyToJump && isGrounded)
        {
            Jump();
            Debug.Log("Jump 1");
            readyToJump = false;
            hasJumped1 = true;    
        }
    }

    private void DoubleJump()
    {
        if (Input.GetKeyDown(jumpKey) && !doubleJumped && !isGrounded) //double jump
        {
            DoubleJumpForce();
            doubleJumped = true;
        }
    }

   
    private void MovePlayer()
    {
        if (activeGrapple) return;
        if (swinging) return;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }
    private void SpeedControl()
    {
        if (activeGrapple) return;

        Vector3 flatvel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if (flatvel.magnitude > moveSpeed)
        {
            Vector3 limitvel = flatvel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitvel.x, rb.velocity.y, limitvel.z);
        }

        if (activeGrapple == true)
        {
            moveSpeed = grappleSpeed;
            Debug.Log("Grapple Speed");
        }
        else
        {
            moveSpeed = groundSpeed;
        }
    }
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void DoubleJumpForce()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * doubleJumpForce, ForceMode.Impulse);
    }
    private bool enableMovementOnNextTouch;
    private void ResetJump()
    {
        readyToJump = true;
        doubleJumped = false;
    }
    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet * grappleSpeed;
    }
    public void JumpToPosition(Vector3 targetPostition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPostition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
    }
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
    public void ResetRestrictions()
    {
        activeGrapple = false;
        readyToJump = true;
        hasJumped1 = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestrictions();
            doubleJumped = false;
            GetComponent<GrapplingNoAIm>().StopGrapple();
        }
        ResetRestrictions();
        doubleJumped = false; 
    }

    public void setGroundedState(bool _grounded)
    {
        isGrounded = _grounded;
    }
}
