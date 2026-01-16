using UnityEngine;

public class Switch : MonoBehaviour
{
    [Header("押し込み対象（見た目：子オブジェクト）")]
    [Tooltip("押されたときに下がる見た目用オブジェクト（未設定なら最初の子を使用）")]
    public Transform pressVisualTarget;

    [Header("押し込み対象（コライダー）")]
    [Tooltip("押されたときに下がるコライダー用オブジェクト")]
    public Transform pressColliderTarget;

    [Header("スイッチ設定")]
    public float pressDepth = 0.2f;
    public float pressSpeed = 5f;

    [Header("表示物（任意）")]
    [Tooltip("スイッチを踏んだら消したいテキストやオブジェクト")]
    public GameObject switchText;

    public bool IsPressed { get; private set; } = false;

    private Vector3 visualInitialPos;
    private Vector3 visualPressedPos;

    private Vector3 colliderInitialPos;
    private Vector3 colliderPressedPos;

    void Start()
    {
        // 見た目が未指定なら「最初の子オブジェクト」を使用
        if (pressVisualTarget == null && transform.childCount > 0)
            pressVisualTarget = transform.GetChild(0);

        if (pressVisualTarget == null)
        {
            Debug.LogWarning("Switch: 見た目用の子オブジェクトが見つかりません");
            pressVisualTarget = transform;
        }

        // コライダー未指定時は動かさない（＝このTransform）
        if (pressColliderTarget == null)
            pressColliderTarget = transform;

        visualInitialPos = pressVisualTarget.position;
        visualPressedPos = visualInitialPos + Vector3.down * pressDepth;

        colliderInitialPos = pressColliderTarget.position;
        colliderPressedPos = colliderInitialPos + Vector3.down * pressDepth;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (IsPressed) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint contact in collision.contacts)
        {
            // 上から踏んだ判定
            if (contact.normal.y < -0.5f)
            {
                Press();
                break;
            }
        }
    }

    void Press()
    {
        IsPressed = true;

        // テキスト削除
        if (switchText != null)
            Destroy(switchText);

        StopAllCoroutines();
        StartCoroutine(MoveDown());
    }

    System.Collections.IEnumerator MoveDown()
    {
        while (
            Vector3.Distance(pressVisualTarget.position, visualPressedPos) > 0.01f ||
            Vector3.Distance(pressColliderTarget.position, colliderPressedPos) > 0.01f
        )
        {
            pressVisualTarget.position = Vector3.Lerp(
                pressVisualTarget.position,
                visualPressedPos,
                Time.deltaTime * pressSpeed
            );

            pressColliderTarget.position = Vector3.Lerp(
                pressColliderTarget.position,
                colliderPressedPos,
                Time.deltaTime * pressSpeed
            );

            yield return null;
        }

        pressVisualTarget.position = visualPressedPos;
        pressColliderTarget.position = colliderPressedPos;
    }
}
