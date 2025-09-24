using UnityEngine;

public class BossBullet : MonoBehaviour
{
    public Boss_20_StatusObjectScript status;
    private float Speed;
    public float LifeTime = 5f;
    public float HitRadious = 5.0f;
    private Transform PlayerTransform;
    private Vector3 MoveDirection;
    private bool Initialized = false;

    public void Initialize(Transform player)
    {
        PlayerTransform = player;
        Speed = status.Attack_Speed;
        MoveDirection = (player.position - transform.position).normalized;
        Initialized = true;

        Destroy(gameObject, LifeTime);
    }

    void Update()
    {
        if (!Initialized) return;
        transform.position += MoveDirection * Speed * Time.deltaTime;
        if(PlayerTransform != null && Vector3.Distance(transform.position, PlayerTransform.position) < HitRadious)
        {
            Debug.Log("ヒット！（自作物理）");
            Destroy(gameObject);
        }
    }
}