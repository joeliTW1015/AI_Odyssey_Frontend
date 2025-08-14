using System.Collections;
using UnityEditor;
using UnityEngine;

public class PlayerTakeDamage : MonoBehaviour
{
    [SerializeField] GameObject HpBar;
    float initHpBarWidth;
    [SerializeField] float maxHp = 100f;
    float currentHp;
    [SerializeField] float repelForce = 5f;
    [SerializeField] float invincibilityDuration = 0.7f;
    bool isInvincible = false;
    [SerializeField] SpriteRenderer playerAnimationSpriteRenderer;
    GameObject weapon;
    Rigidbody2D rb;
    PlayerMove playerMove;
    void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        rb = GetComponent<Rigidbody2D>();
        weapon = GetComponentInChildren<PlayerWeponHandler>().gameObject;
        if (HpBar != null)
        {
            initHpBarWidth = HpBar.transform.localScale.x;
        }
        currentHp = maxHp;
    }

    public void TakeDamage(float damage, Vector2 damageSourcePosition)
    {
        if (isInvincible) return; // Ignore damage if invincible
        playerMove.enabled = false; // Disable player movement while taking damage
        rb.linearVelocity = Vector2.zero; // Stop player movement immediately
        currentHp -= damage;
        HpBar.transform.localScale = new Vector3(initHpBarWidth * (currentHp / maxHp), HpBar.transform.localScale.y, HpBar.transform.localScale.z);
        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
        else
        {
            //TODO: Add damage animation
            StartCoroutine(InvincibilityCoroutine());
            rb.AddForce((transform.position - (Vector3)damageSourcePosition).normalized * repelForce, ForceMode2D.Impulse);
        }
    }

    private void Die()
    {
        // Handle player death (e.g., play animation, disable controls, etc.)
        Debug.Log("Player has died.");
        playerMove.enabled = false; // Disable player movement
        weapon.SetActive(false); // Disable weapon
        //TODO: Add death animation or effects and call a game over function
    }

    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;
        playerMove.enabled = false; // Disable player movement during invincibility
        //閃爍5次
        for (int i = 0; i < 5; i++)
        {
            playerAnimationSpriteRenderer.enabled = false;
            yield return new WaitForSeconds(invincibilityDuration / 10);
            playerAnimationSpriteRenderer.enabled = true;
            yield return new WaitForSeconds(invincibilityDuration / 10);
        }
        isInvincible = false;
        playerMove.enabled = true; // Re-enable player movement after invincibility
        playerAnimationSpriteRenderer.enabled = true; // Ensure the sprite is visible after invincibility
    }



}
