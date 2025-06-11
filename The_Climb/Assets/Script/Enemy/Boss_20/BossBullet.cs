using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public Boss_20_StatusObjectScript status;
    private float speed;
    public float lifeTime = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Bullet()
    {
        speed = status.Attack_Speed;
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }
}
