using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector3 _move;
    private Vector3 _gravityVelocity;
    private Transform _cam;
    [NonSerialized] public CharacterController _controller;
    
    private float turnSmoothVelocity = 0f;
    private float turnSmoothTime = 0.1f;
    private float speed = 5f;
    public bool isGrounded;
    public bool gotBumped;
    private float gravity = -9.81f;
    public float jumpHeight;
    
    [SerializeField]private Transform groundCheck;
    public LayerMask groundMask;
    
    [NonSerialized] public Vector3 _getSmashedVelocity;
    [NonSerialized] public Vector3 _jumpSmashVelocity;
    
    private void Awake()
    {
        _cam = Camera.main.transform;
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        RegularMovement();
    }
    
    void RegularMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.5f, groundMask);
        
        if(isGrounded && _gravityVelocity.y < 0)
        {
            _gravityVelocity.y = -2f;
            gotBumped = false;
            _jumpSmashVelocity = Vector3.zero;
        }
        
        float z = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");

        _move = _cam.right * x + _cam.forward * z;
        _move = _move.normalized;
        _move.y = -0.1f;
        
        if (!gotBumped) _controller.Move(_move * speed * Time.deltaTime);
        
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump(jumpHeight);
        }

        if (_move.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(_move.x, _move.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
        
        //Gravity
        
        _gravityVelocity.y += gravity * Time.deltaTime;
        _controller.Move(_gravityVelocity * Time.deltaTime);
        
        // Enemy bumping
        _controller.Move(_getSmashedVelocity * Time.deltaTime);
        _controller.Move(_jumpSmashVelocity * Time.deltaTime);
    }
    
    public void Jump(float jumpHeight)
    {
        _gravityVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
    }

    
    public Vector3 GetMove()
    {
        return _move;
    }
}
