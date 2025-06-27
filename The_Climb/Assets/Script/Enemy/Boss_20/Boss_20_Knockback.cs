using UnityEngine;
using System.Collections;
public class Boss_20_Knockback : MonoBehaviour
{

    [SerializeField] public Transform player;       // プレイヤーのTransform
    [SerializeField] public float knockbackDistance = 5f; // 発動距離
    [SerializeField] public float knockbackForce = 0.1f;    // ノックバックの力
    [SerializeField] private float stopDuration = 0.2f;        // 停止している時間
                     private bool isKnockbacking = false;                  // 移動を一時的に止めるフラグ

    private Rigidbody rb;
    private bool hasKnockedBack = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
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

        // 一定以上離れたらノックバックを再び許可
        if (distance > knockbackDistance + 1f)
        {
            hasKnockedBack = false;
        }
    }

    void ApplyKnockback()
    {
        Debug.Log("ノックバック発動！");
        Vector3 direction = (transform.position - player.position).normalized;
        rb.AddForce(direction * knockbackForce, ForceMode.Impulse);

        StartCoroutine(ResetVelocityAfterKnockback());
    }

    IEnumerator ResetVelocityAfterKnockback()
    {
        yield return new WaitForSeconds(knockbackDistance); // 少し時間を置く（力が加わった直後）

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero; // 回転も止めたい場合

        yield return new WaitForSeconds(stopDuration);

        isKnockbacking = false;
    }
}
