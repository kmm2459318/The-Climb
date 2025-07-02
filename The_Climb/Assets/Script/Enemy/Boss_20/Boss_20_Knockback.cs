using UnityEngine;
using System.Collections;

public class Boss_20_Knockback : MonoBehaviour
{
    [SerializeField] public Transform player;
    [SerializeField] public float knockbackDistance = 5f;
    [SerializeField] public float knockbackForce = 0.1f;
    [SerializeField] private float knockbackDelay = 0.1f;
    [SerializeField] private float stopDuration = 0.2f;

    private Rigidbody rb;
    private bool isKnockbacking = false;
    private bool hasKnockedBack = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isKnockbacking) return;

        CheckPlayerDistance();
    }

    void CheckPlayerDistance()
    {
        if (player == null || rb == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= knockbackDistance && !hasKnockedBack)
        {
            ApplyKnockback();
            hasKnockedBack = true;
        }

        if (distance > knockbackDistance + 1f)
        {
            hasKnockedBack = false;
        }
    }

    void ApplyKnockback()
    {
        Debug.Log("ノックバック発動！");
        isKnockbacking = true;

        Vector3 direction = (transform.position - player.position).normalized;
        rb.AddForce(direction * knockbackForce, ForceMode.Impulse);

        StartCoroutine(ResetVelocityAfterKnockback());
    }

    IEnumerator ResetVelocityAfterKnockback()
    {
        yield return new WaitForSeconds(knockbackDelay);

        rb.angularVelocity = Vector3.zero;

        yield return new WaitForSeconds(stopDuration);

        isKnockbacking = false;
    }
}