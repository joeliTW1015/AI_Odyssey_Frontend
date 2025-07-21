using UnityEngine;

public class BulletBase : MonoBehaviour
{
    protected bool isInitialized = false; // Flag to check if the bullet has been initialized
    protected Rigidbody2D rb;
    protected float bulletSpeed;
    protected float fireRange;
    protected float damage;
    Vector2 initialPosition;


    public void Initialize(float bulletSpeed, float fireRange, float damage)
    {
        this.bulletSpeed = bulletSpeed;
        this.fireRange = fireRange;
        this.damage = damage;
        initialPosition = transform.position;
        isInitialized = true;
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D component not found on BulletBase.");
        }
    }

    protected virtual void Update()
    {
        if (!isInitialized)
        {
            return;
        }

        // Move the bullet forward based on its speed
        rb.linearVelocity = transform.up * bulletSpeed;

        // Destroy the bullet after a certain range
        if (Vector2.Distance(transform.position, initialPosition) > fireRange)
        {
            BulletDestroy();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            BulletDestroy();
        }
        else if (other.CompareTag("Walls"))
        {
            BulletDestroy();
        }
    }

    protected virtual void BulletDestroy()
    {
        // TEMP
        Destroy(gameObject);
    }
    
}
