using UnityEngine;
using System.Collections;

public class Boss_20_Knockback : MonoBehaviour
{
    [SerializeField] public Transform player;
    [SerializeField] public float knockbackDistance = 5f;
    [SerializeField] public float upwardHeight = 2f;     // 上昇の高さ
    [SerializeField] public float backwardDistance = 2f; // 後退の距離
    [SerializeField] public float knockbackDuration = 0.5f;
    [SerializeField] public float returnDuration = 0.5f;

    private bool isKnockbacking = false;
    private bool hasKnockedBack = false;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (isKnockbacking) return;
        CheckPlayerDistance();
    }

    void CheckPlayerDistance()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= knockbackDistance && !hasKnockedBack)
        {
            StartCoroutine(PerformKnockback());
            hasKnockedBack = true;
        }

        if (distance > knockbackDistance + 1f)
        {
            hasKnockedBack = false;
        }
    }

    IEnumerator PerformKnockback()
    {
        isKnockbacking = true;

        Vector3 knockDirection = (transform.position - player.position).normalized;
        Vector3 upwardTarget = transform.position + Vector3.up * upwardHeight;
        Vector3 backwardTarget = upwardTarget + knockDirection * backwardDistance;

        // 上昇しながら後退（ノックバック）
        float elapsed = 0f;
        Vector3 start = transform.position;

        while (elapsed < knockbackDuration)
        {
            transform.position = Vector3.Lerp(start, backwardTarget, elapsed / knockbackDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = backwardTarget;

        // 元のY座標にふわっと戻る
        Vector3 fallTarget = new Vector3(transform.position.x, originalPosition.y, transform.position.z);
        elapsed = 0f;
        Vector3 fallStart = transform.position;

        while (elapsed < returnDuration)
        {
            transform.position = Vector3.Lerp(fallStart, fallTarget, elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = fallTarget;

        isKnockbacking = false;
    }
}
