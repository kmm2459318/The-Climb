using UnityEngine;

public class NormalBullet : BulletBase
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
    }

    ///<summy>
    ///銃の出る方向
    ///</summy>
    public override void Shoot(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y ,direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    //消す処理
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Destroy(other.gameObject);
        }
    }
}
