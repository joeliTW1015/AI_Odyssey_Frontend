using UnityEngine;
using System.Collections;
public abstract class EnemyBase : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float speed;
    [SerializeField] protected float attackDamage;
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float repelForce = 3f; // Force applied to the enemy when it takes damage
    [SerializeField] float repelTime = 0.5f; // 硬直
    [SerializeField] float deadFadeDuration = 0.5f; // Duration of the fade out animation when the enemy dies

    [SerializeField] GameObject healthBarObject;
    [SerializeField] protected float spawnAnimationDuration = 0.3f; // Duration of the spawn animation
    protected float health;
    protected Vector2 targetPosition;

    float initHealthBarScaleX;
    protected Rigidbody2D rb;

    bool isSpawning = false; // Flag to indicate if the enemy is currently spawning
    protected bool canMove;
    SpriteRenderer animationSpriteRenderer;

    protected virtual void Awake()
    {
        animationSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        health = maxHealth;
        initHealthBarScaleX = healthBarObject.transform.localScale.x;
        canMove = true; // Enable movement by default
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on EnemyBase.");
        }
    }

    protected abstract void GetTargetPosition();

    //生成時的動畫
    protected virtual void OnEnable()
    {
        isSpawning = true;
        StartCoroutine(SpawnAnimation());
    }

    private IEnumerator SpawnAnimation()
    {
        //from transparent to visible, from black to normal color
        float elapsedTime = 0f;
        Color initialColor = new Color(0f, 0f, 0f, 0f); // Start with fully transparent black
        Color targetColor = Color.white; // Target color is white
        targetColor.a = 1f; // Set target color alpha to fully opaque
        animationSpriteRenderer.color = initialColor; // Set initial color
        while (elapsedTime < spawnAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / spawnAnimationDuration;
            animationSpriteRenderer.color = Color.Lerp(initialColor, targetColor, t);
            yield return null; // Wait for the next frame
        }
        isSpawning = false; // Set the flag to false after the spawn animation is done
    }



    protected virtual void Update()
    {
        if (isSpawning || health <= 0)
        {
            return; // Skip update if the enemy is currently spawning
        }
        //更新目標位置
        GetTargetPosition();
        //移動
        if (canMove && rb != null)
        {
            MoveTowardsTarget();
        }
        
    }
    protected virtual void MoveTowardsTarget()
    {
        rb.linearVelocity = (targetPosition - rb.position).normalized * speed;
        if (rb.linearVelocity.x < -0.01f)
        {
            animationSpriteRenderer.flipX = true;
        }
        else if (rb.linearVelocity.x > 0.01f)
        {
            animationSpriteRenderer.flipX = false;
        }
    }

    public virtual void TakeDamage(float damage, Vector2 repelDirection)
    {
        if (isSpawning || health <= 0) 
        {
            return; // Ignore damage if the enemy is currently spawning or already dead
        }

        health -= damage;
        if (healthBarObject != null)
        {
            Vector3 healthBarScale = healthBarObject.transform.localScale;
            healthBarScale.x = initHealthBarScaleX * (health / maxHealth);
            healthBarObject.transform.localScale = healthBarScale;
        }
        rb.linearVelocity = repelDirection.normalized * repelForce; // Apply repel force
        canMove = false; // Disable movement while taking damage
        StartCoroutine(TakeDamageAnimation()); // Start damage animation
    }

    IEnumerator TakeDamageAnimation()
    {
        canMove = false; // Disable movement during damage animation
        //閃爍5次
        for (int i = 0; i < 5; i++)
        {
            if (animationSpriteRenderer == null)
            {
                yield break; // Exit if the sprite renderer is not found
            }
            animationSpriteRenderer.color = Color.red; // Change color to red
            yield return new WaitForSeconds(repelTime / 10); // Wait for a short duration
            animationSpriteRenderer.color = Color.white; // Reset color to white
            yield return new WaitForSeconds(repelTime / 10); // Wait for a short duration
        }

        if (health <= 0)
        {
            Die();
        }
        canMove = true; // Re-enable movement after the damage animation
    }

    protected virtual void Die()
    {
        // Logic for when the enemy dies

        transform.parent.GetComponentInParent<DungeonRoom>().remainingEnemies--; // Decrease the count of remaining enemies in the parent DungeonRoom
        Debug.Log($"{gameObject.name} has died. Remaining enemies: {transform.parent.GetComponentInParent<DungeonRoom>().remainingEnemies}");
        rb.linearVelocity = Vector2.zero; // Stop movement immediately
        GetComponent<Collider2D>().enabled = false; // Disable the collider to prevent further interactions
        StartCoroutine(DieFadeAnimation()); // Start the fade animation before destroying the enemy
    }

    IEnumerator DieFadeAnimation()
    {
        canMove = false; // Disable movement
        float elapsedTime = 0f;
        Color initialColor = animationSpriteRenderer.color; // Start with the current color
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f); // Target color is fully transparent
        while (elapsedTime < deadFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / deadFadeDuration;
            animationSpriteRenderer.color = Color.Lerp(initialColor, targetColor, t);
            yield return null; // Wait for the next frame
        }
        gameObject.SetActive(false); // Deactivate the enemy
    }
}
