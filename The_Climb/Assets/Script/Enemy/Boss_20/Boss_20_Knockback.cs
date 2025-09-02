using UnityEngine;
using System.Collections;


public class Boss_20_Knockback : MonoBehaviour
{
    [SerializeField] public Transform Player;
    [SerializeField] public float KnockbackDistance = 5f;
    [SerializeField] public float UpwardHeight = 2f;     // 上昇の高さ
    [SerializeField] public float BackwardDistance = 2f; // 後退の距離
    [SerializeField] public float KnockbackDuration = 0.5f;
    [SerializeField] public float ReturnDuration = 0.5f;
    [SerializeField] private float YThreshold = 0.5f;        // ボスより下からぶつかってきたと判断する高さ差

    public bool IsKnockbacking { get; private set; } = false;
    private bool HasKnockedBack = false;
    private float BaseY;
    private void Start()
    {
        BaseY = transform.position.y;
    }


    void Update()
    {
        if (IsKnockbacking) return;
        CheckPlayerDistance();
    }

    void CheckPlayerDistance()
    {
        if (Player == null) return;

        float distance = Vector3.Distance(transform.position, Player.position);
        // 2. Y軸の差（ボスが高いほど正）
        float yDiff = transform.position.y - Player.position.y;

        // === 条件まとめ ===
        bool isClose = distance <= KnockbackDistance;
        bool isFromBelow = yDiff > YThreshold;  // プレイヤーがボスより一定以上下にいる

        if (isClose && isFromBelow && !HasKnockedBack)
        {
            StartCoroutine(PerformKnockback());
            HasKnockedBack = true;
        }

        if (distance > KnockbackDistance + 1f)
        {
            HasKnockedBack = false;
        }
    }


    IEnumerator PerformKnockback()
    {
        IsKnockbacking = true;

        float fixedZ = transform.position.z;

        // ノックバック方向（X軸のみ）
        Vector3 direction = transform.position - Player.position;
        direction.y = 0f;
        direction.z = 0f;
        Vector3 knockDirection = direction.normalized;

        // ノックバックの目標位置
        Vector3 knockTarget = new Vector3(
            transform.position.x + knockDirection.x * BackwardDistance,
            BaseY + UpwardHeight, // ← 毎回同じ「地面からの高さ」
            fixedZ
        );

        float elapsed = 0f;
        Vector3 startPos = new Vector3(transform.position.x, BaseY, fixedZ);

        while (elapsed < KnockbackDuration)
        {
            transform.position = Vector3.Lerp(startPos, knockTarget, elapsed / KnockbackDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = knockTarget;

        // 落下
        Vector3 fallStart = transform.position;
        Vector3 fallTarget = new Vector3(knockTarget.x, BaseY, fixedZ);
        elapsed = 0f;

        while (elapsed < ReturnDuration)
        {
            transform.position = Vector3.Lerp(fallStart, fallTarget, elapsed / ReturnDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = fallTarget;
        IsKnockbacking = false;
    }
}