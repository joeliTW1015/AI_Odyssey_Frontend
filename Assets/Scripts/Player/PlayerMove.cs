using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    Vector2 moveDirection;
    SpriteRenderer spriteRenderer;
    [SerializeField] FixedJoystick moveVirtualJoystick; // For mobile controls, if needed
    [SerializeField] InputActionReference moveAction;
    [SerializeField] float moveSpeed = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

    }

    void Move()
    {
        moveDirection = Vector2.zero;
        moveDirection += new Vector2(moveVirtualJoystick.Horizontal, moveVirtualJoystick.Vertical).normalized;
        //moveDirection += moveAction.action.ReadValue<Vector2>();
        if (moveDirection.magnitude < 0.01f)
        {
            moveDirection = Vector2.zero; // Prevent small movements
        }
        
        rb.linearVelocity = moveDirection * moveSpeed;
        animator.SetFloat("speed", moveDirection.magnitude);
        // Flip the player sprite based on the direction of movement
        if (moveDirection.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveDirection.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
}
