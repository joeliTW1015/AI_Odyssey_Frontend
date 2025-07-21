using System.Collections;
using UnityEngine;

public class Pistol : WeponBase
{

    IEnumerator Fire()
    {
        isFiring = true;
        
        // Instantiate the bullet at the fire point
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.Euler(0, 0, firePoint.rotation.eulerAngles.z - 90f)); //四元数相乘
        bullet.GetComponent<BulletBase>().Initialize(bulletSpeed, fireRange, damage);
        //TEST ，震动反馈
        Handheld.Vibrate();

        //用插值做槍枝後座力的動畫
        float elapsedTime = 0f;
        Vector3 initialPosition = weaponAnimationObject.transform.localPosition;
        Vector3 targetPosition = initialPosition + new Vector3(-0.1f, 0, 0); // 後座力的目標位置 
        while (elapsedTime < fireRate / 2)
        {
            weaponAnimationObject.transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / (fireRate / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        weaponAnimationObject.transform.localPosition = targetPosition; // 确保到达目标位置

        elapsedTime = 0f;
        // Return to the initial position
        while (elapsedTime < fireRate / 2)
        {
            weaponAnimationObject.transform.localPosition = Vector3.Lerp(targetPosition, initialPosition, (elapsedTime - fireRate / 2) / (fireRate / 2));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        weaponAnimationObject.transform.localPosition = initialPosition; // 确保回到初始位置
        
        isFiring = false;
    }

    protected override void Awake()
    {
        base.Awake();
        // Additional pistol-specific initialization if needed
    }

    protected override void Start()
    {
        base.Start();
        // Additional pistol-specific start logic if needed
    }

    protected override void Update()
    {
        base.Update();
        // Additional pistol-specific update logic if needed
        if(playerWeponHandler.joystickMagnitude > 0.1f && !isFiring)
        {
            StartCoroutine(Fire());
        }
    }
}
