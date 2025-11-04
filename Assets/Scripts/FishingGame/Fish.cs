using System;
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
    public int predictType; // 0: silverfish, 1: normalfish
    public float confidenceScore; //預測的信心分數 <0.5 表示錯誤預測
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
        transform.position = new Vector2(UnityEngine.Random.Range(-targetRangeX, targetRangeX), UnityEngine.Random.Range(targetRangeYMin, targetRangeYMax));
        targetPosition = new Vector2(UnityEngine.Random.Range(-targetRangeX, targetRangeX), UnityEngine.Random.Range(targetRangeYMin, targetRangeYMax));
        spriteRenderer.sprite = unknownFishSprite; // Set default sprite to unknown fish
        confidenceScore = 0.5f + (LevelManager_02.trainingAccuracy / 30)  / 2f + UnityEngine.Random.Range(-0.2f, 0.2f);// Confidence score between 0 and 1.0
        if (confidenceScore > 1f)
        {
            confidenceScore = 1f;
        }
        else if (confidenceScore < 0f)
        {
            confidenceScore = 0f;
        }
        Debug.Log($"Fish {fishIndex} of type {fishType} has confidence score {confidenceScore}");
        if (confidenceScore >= 0.5f)
        {
            predictType = fishType; // correct prediction
        }
        else
        {
            predictType = 1 - fishType; // incorrect prediction
        }

        StartCoroutine(FlashSprite());
    }

    IEnumerator FlashSprite()
    {
        while (!isCatched)
        {
            spriteRenderer.sprite = unknownFishSprite;
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f); //opaque
            float waitingTime = UnityEngine.Random.Range(0.2f, 0.5f);
            yield return new WaitForSeconds(waitingTime);
            spriteRenderer.sprite = predictType == 0 ? silverFishSprite : normalFishSprite;
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f); //半透明
            yield return new WaitForSeconds(waitingTime);
        }
    }
    public void ReStartFlash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashSprite());
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
            targetPosition = new Vector2(UnityEngine.Random.Range(-targetRangeX, targetRangeX), UnityEngine.Random.Range(targetRangeYMin, targetRangeYMax));
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
