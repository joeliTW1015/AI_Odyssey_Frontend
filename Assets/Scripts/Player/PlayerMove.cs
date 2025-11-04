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

    //bool isMobile = false;

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
        //isMobile = SystemInfo.deviceType == DeviceType.Handheld;
        //// 額外檢查平板（螢幕大於手機但小於桌機）
        //float aspect = (float)Screen.width / Screen.height;
        //if (!isMobile && Screen.width < 1600 && aspect < 1.8f)
        //{
        //    isMobile = true; // 很可能是平板
        //}

        //moveVirtualJoystick.gameObject.SetActive(isMobile);
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
        
        //使用虛擬搖桿控制
        moveDirection += new Vector2(moveVirtualJoystick.Horizontal, moveVirtualJoystick.Vertical).normalized;
        
        
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
