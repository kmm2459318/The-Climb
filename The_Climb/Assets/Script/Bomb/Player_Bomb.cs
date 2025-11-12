using UnityEngine;

public class Player_Bomb : MonoBehaviour
{
    [SerializeField] ParticleSystem b_particle;
    [SerializeField] float b_force = 10;
    [SerializeField] float b_radius = 5;
    [SerializeField] float b_upward = 0;
    [SerializeField] float b_time = 3;

    private float b_explosion = 0;
    private bool exploded = false;
    private Vector3 b_pos;

    void Update()
    {
        b_explosion += Time.deltaTime; 

        if (b_explosion >= b_time && !exploded)
        {
            exploded = true;
            Explosion();
        }
    }

    void Explosion()
    {
        b_pos = transform.position;

        if (b_particle != null)
            b_particle.Play();

        Collider[] hitColliders = Physics.OverlapSphere(b_pos, b_radius);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            var obj = hitColliders[i].gameObject;
            var rb = hitColliders[i].GetComponent<Rigidbody>();
            if (rb)
            {
                rb.AddExplosionForce(b_force, b_pos, b_radius, b_upward, ForceMode.Impulse);
                if (obj.CompareTag("BreakingWall"))
                {
                    Destroy(obj);
                    Debug.Log($"{obj.name}を破棄しました");
                }
            }
        }

        Debug.Log("爆発しました");
        Destroy(gameObject); // 1秒後に削除
    }
}