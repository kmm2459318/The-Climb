using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Boss_20_Knockback : MonoBehaviour
{
    [SerializeField] public Transform player;
    [SerializeField] public float knockbackDistance = 5f;
    [SerializeField] public float upwardHeight = 2f;     // 上昇の高さ
    [SerializeField] public float backwardDistance = 2f; // 後退の距離
    [SerializeField] public float knockbackDuration = 0.5f;
    [SerializeField] public float returnDuration = 0.5f;
    [SerializeField] private float yThreshold = 0.5f;        // ボスより下からぶつかってきたと判断する高さ差

    public bool IsKnockbacking { get; private set; } = false;
    private bool hasKnockedBack = false;
    private float baseY;
    private void Start()
    {
        baseY = transform.position.y;
    }


    void Update()
    {
        if (IsKnockbacking) return;
        CheckPlayerDistance();
    }

    void CheckPlayerDistance()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        // 2. Y軸の差（ボスが高いほど正）
        float yDiff = transform.position.y - player.position.y;

        // === 条件まとめ ===
        bool isClose = distance <= knockbackDistance;
        bool isFromBelow = yDiff > yThreshold;  // プレイヤーがボスより一定以上下にいる

        if (isClose && isFromBelow && !hasKnockedBack)
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
        IsKnockbacking = true;

        float fixedZ = transform.position.z;

        // ノックバック方向（X軸のみ）
        Vector3 direction = transform.position - player.position;
        direction.y = 0f;
        direction.z = 0f;
        Vector3 knockDirection = direction.normalized;

        // ノックバックの目標位置
        Vector3 knockTarget = new Vector3(
            transform.position.x + knockDirection.x * backwardDistance,
            baseY + upwardHeight, // ← 毎回同じ「地面からの高さ」
            fixedZ
        );

        float elapsed = 0f;
        Vector3 startPos = new Vector3(transform.position.x, baseY, fixedZ);

        while (elapsed < knockbackDuration)
        {
            transform.position = Vector3.Lerp(startPos, knockTarget, elapsed / knockbackDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = knockTarget;

        // 落下
        Vector3 fallStart = transform.position;
        Vector3 fallTarget = new Vector3(knockTarget.x, baseY, fixedZ);
        elapsed = 0f;

        while (elapsed < returnDuration)
        {
            transform.position = Vector3.Lerp(fallStart, fallTarget, elapsed / returnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = fallTarget;
        IsKnockbacking = false;
    }
}