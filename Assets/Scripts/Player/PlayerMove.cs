using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    Vector2 moveDirection;
    SpriteRenderer spriteRenderer;
    public static bool canMove = true;
    [SerializeField] FixedJoystick moveVirtualJoystick; // For mobile controls, if needed
    [SerializeField] float moveSpeed = 5f;

    void Awake()
    {
        canMove = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //判斷裝置是否為平板或手機
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            //啟用虛擬搖桿
            moveVirtualJoystick.gameObject.SetActive(true);
        }
        else
        {
            //關閉虛擬搖桿
            moveVirtualJoystick.gameObject.SetActive(false);
        }
    }

    void Move()
    {
        moveDirection = Vector2.zero;
        if(!canMove) // Disable movement
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetFloat("speed", 0f);
            return;
        }


        if (Keyboard.current != null)
        {
            //使用鍵盤控制
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
            {
                moveDirection += Vector2.up;
            }
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
            {
                moveDirection += Vector2.down;
            }
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                moveDirection += Vector2.left;
            }
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                moveDirection += Vector2.right;
            }
        }
        else
        {
            //使用虛擬搖桿控制
            moveDirection += new Vector2(moveVirtualJoystick.Horizontal, moveVirtualJoystick.Vertical).normalized;
        }
        
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
