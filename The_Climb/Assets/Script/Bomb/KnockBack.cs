using UnityEngine;

public class KnockBack : MonoBehaviour
{
    Rigidbody rb;
    bool isKnockback = false;
    Vector3 knockVelocity;

    [SerializeField] float knockDuration = 0.15f;
    float knockTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!isKnockback) return;

        knockTimer -= Time.fixedDeltaTime;
        rb.MovePosition(rb.position + knockVelocity * Time.fixedDeltaTime);

        if (knockTimer <= 0f)
        {
            isKnockback = false;
            knockVelocity = Vector3.zero;
        }
    }

    // 爆風を受ける
    public void ApplyBombKnockback(Vector3 explosionPos, float power, float radius)
    {
        Vector3 dir = transform.position - explosionPos;
        float distance = dir.magnitude;

        if (distance > radius) return;

        dir.Normalize();

        float rate = 1f - (distance / radius); 
        knockVelocity = dir * power * rate;

        rb.linearVelocity = Vector3.zero;
        isKnockback = true;
        knockTimer = knockDuration;
    }
}
