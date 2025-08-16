using System.Collections;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float turningSpeed = 2f;
    //range of target position
    [SerializeField] float targetRangeX = 10f;
    [SerializeField] float targetRangeYMin = 5f;
    [SerializeField] float targetRangeYMax = 10f;
    [Header("Fish Info")]
    public int fishIndex;
    public int fishType; // 0: silverfish, 1: normalfish
    [SerializeField] Sprite unknownFishSprite;
    [SerializeField] Sprite silverFishSprite;
    [SerializeField] Sprite normalFishSprite;

    Rigidbody2D rb;
    Collider2D coll;
    public bool isCatched = false;
    SpriteRenderer spriteRenderer;
    Vector2 targetPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isCatched = false;
        transform.position = new Vector2(Random.Range(-targetRangeX, targetRangeX), Random.Range(targetRangeYMin, targetRangeYMax));
        targetPosition = new Vector2(Random.Range(-targetRangeX, targetRangeX), Random.Range(targetRangeYMin, targetRangeYMax));
        spriteRenderer.sprite = unknownFishSprite; // Set default sprite to unknown fish
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCatched)
        {
            MoveTowardsTarget();
        }
    }

    public void DisablePhysics()
    {
        rb.linearVelocity = Vector2.zero;
        coll.enabled = false;
        //stop rotation
        rb.angularVelocity = 0f;
    }
    public void EnablePhysics()
    {
        coll.enabled = true;
    }

    void MoveTowardsTarget()
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.AddForce(direction * speed * Time.deltaTime, ForceMode2D.Force);

        if (Vector2.Distance(transform.position, targetPosition) < 0.2f)
        {
            targetPosition = new Vector2(Random.Range(-targetRangeX, targetRangeX), Random.Range(targetRangeYMin, targetRangeYMax));
        }
        float targetRotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float currentRotation = transform.eulerAngles.z;
        float newRotation = Mathf.MoveTowardsAngle(currentRotation, targetRotation, turningSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, newRotation);
        //flip sprite based on direction
        if (direction.x < 0)
        {
            spriteRenderer.flipY = true;
        }
        else
        {
            spriteRenderer.flipY = false;
        }
    }
    
    public void ChangeSprite()
    {
        switch (fishType)
        {
            case 0:
                spriteRenderer.sprite = silverFishSprite;
                break;
            case 1:
                spriteRenderer.sprite = normalFishSprite;
                break;
            default:
                spriteRenderer.sprite = unknownFishSprite;
                break;
        }
    }

    
}
