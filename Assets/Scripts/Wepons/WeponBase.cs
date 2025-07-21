using UnityEngine;

public abstract class WeponBase : MonoBehaviour //加上abstract关键字，表示这是一个抽象类，
{
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected GameObject weaponAnimationObject; 
    public float bulletSpeed = 10f; // Speed of the bullet
    [SerializeField] protected Transform firePoint;
    public float fireRate = 0.5f; // Time between shots
    public float fireRange = 10f; // Range of the bullet
    public float damage = 10f; // Damage dealt by the bullet
    protected bool isFiring = false;
    protected PlayerWeponHandler playerWeponHandler;

    protected virtual void Awake()
    {
        playerWeponHandler = FindFirstObjectByType<PlayerWeponHandler>();
        isFiring = false;
    }

    
    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    
    
}
