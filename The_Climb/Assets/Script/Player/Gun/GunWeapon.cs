using UnityEngine;

public class GunWeapon : WeaponBase
{

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            TryShoot();
        }
    }

    private void TryShoot()
    {
        if (Time.time >= NextFireTime)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = (mousePos - FirePos.position).normalized;

            Fire(direction);
            NextFireTime = Time.time + FireRate;
        }
    }

    public override void Fire(Vector3 direction)
    {
        GameObject bullet = Instantiate(BulletPrefab, FirePos.position, Quaternion.identity);
        BulletBase bulletComp= bullet.GetComponent<BulletBase>();
        bulletComp.Shoot(direction);
    }
}

