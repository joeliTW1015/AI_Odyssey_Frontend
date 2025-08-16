using UnityEngine;

public class Hook : MonoBehaviour
{
    public Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fish"))
        {
            FishingGameManager.Instance.OnFishCaught(collision.gameObject);
        }
        else if (collision.CompareTag("Walls"))
        {
            FishingGameManager.Instance.OnHookHitWall();
        }
    }
}
