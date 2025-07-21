using UnityEngine;

public class BasicBullet : BulletBase
{
    protected override void Awake()
    {
        base.Awake();
        // Additional initialization for BasicBullet if needed
    }

    protected override void Update()
    {
        base.Update();
        // Additional update logic for BasicBullet if needed
    }

    protected override void BulletDestroy()
    {
        // Custom destruction logic for BasicBullet if needed
        base.BulletDestroy();
    }
}
