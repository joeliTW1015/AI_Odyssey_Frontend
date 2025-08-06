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
    public int fishIndex = 0;

    Rigidbody2D rb;
    public bool isCatched = false;
    SpriteRenderer spriteRenderer;
    Vector2 targetPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        isCatched = false;
        transform.position = new Vector2(Random.Range(-targetRangeX, targetRangeX), Random.Range(targetRangeYMin, targetRangeYMax));
        targetPosition = new Vector2(Random.Range(-targetRangeX, targetRangeX), Random.Range(targetRangeYMin, targetRangeYMax));
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCatched)
        {
            MoveTowardsTarget();
        }
    }

    void MoveTowardsTarget()
    {
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.AddForce(direction * speed * Time.deltaTime, ForceMode2D.Force);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
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
    

    
}
