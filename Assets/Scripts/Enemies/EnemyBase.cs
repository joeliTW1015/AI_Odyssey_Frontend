using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
    protected Rigidbody2D rb;
    [SerializeField] protected float maxHealth;
    protected float health;
    [SerializeField] protected float speed;
    [SerializeField] GameObject healthBarObject;
    public float enemyDamage;
    protected Vector2 targetPosition;

    float initHealthBarScaleX;
    [SerializeField] SpriteRenderer animationSpriteRenderer;

    protected virtual void Awake()
    {
        health = maxHealth;
        initHealthBarScaleX = healthBarObject.transform.localScale.x;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on EnemyBase.");
        }
    }

    protected abstract void GetTargetPosition();


    protected virtual void Update()
    {
        //更新目標位置
        GetTargetPosition();
        //移動
        rb.linearVelocity = (targetPosition - rb.position).normalized * speed;
        if (rb.linearVelocity.x < -0.01f)
        {
            animationSpriteRenderer.flipX = true;
        }
        else if (rb.linearVelocity.x > 0.01f)
        {
            animationSpriteRenderer.flipX = false;
        }

        //hp bar update
        if (healthBarObject != null)
        {
            Vector3 healthBarScale = healthBarObject.transform.localScale;
            healthBarScale.x = initHealthBarScaleX * (health / maxHealth);
            healthBarObject.transform.localScale = healthBarScale;
        }
    }

    public void TakeDamage(float damage)
    {

        health -= damage;
        if (health <= 0)
        {
            Die();
        }

        //受到傷害動畫

    }

    protected virtual void Die()
    {
        // Logic for when the enemy dies
        Destroy(gameObject);
    }
}
