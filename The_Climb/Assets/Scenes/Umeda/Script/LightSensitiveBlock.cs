using UnityEngine;

/// <summary>
/// 光に反応して出現・消失する足場ブロック。
/// DetectionTrigger は統合済み。
/// </summary>
[DisallowMultipleComponent]
public class LightSensitiveBlock : MonoBehaviour
{
    [Header("感知用Collider（子オブジェクトなどに配置）")]
    [Tooltip("光を検知するためのTrigger Colliderを設定します。")]
    public Collider detectionCollider;

    [Header("反応するレイヤー設定")]
    [Tooltip("光として反応させたいレイヤーを指定します。")]
    public LayerMask detectableLayers;

    private Renderer blockRenderer;
    private Collider blockCollider;
    private int lightCount = 0; // 何個のライトに照らされているか

    void Start()
    {
        blockRenderer = GetComponent<Renderer>();
        blockCollider = GetComponent<Collider>();
        ActivateBlock(false);

        if (detectionCollider == null)
        {
            Debug.LogWarning($"[LuminaBlock] 感知用Colliderが設定されていません: {name}");
            return;
        }

        // 必要なら Rigidbody を追加（Trigger Collider の OnTrigger が動くため）
        Rigidbody rb = detectionCollider.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = detectionCollider.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        // このオブジェクトに OnTrigger を直接追加
        TriggerForwarder forwarder = detectionCollider.gameObject.AddComponent<TriggerForwarder>();
        forwarder.targetBlock = this;
        forwarder.detectableLayers = detectableLayers;
    }

    public void ActivateBlock(bool active)
    {
        if (blockRenderer != null)
            blockRenderer.enabled = active;

        if (blockCollider != null)
            blockCollider.enabled = active;
    }

    public void AddLight()
    {
        lightCount++;
        ActivateBlock(true);
    }

    public void RemoveLight()
    {
        lightCount = Mathf.Max(0, lightCount - 1);
        if (lightCount == 0)
        {
            ActivateBlock(false);
        }
    }

    public void ForceDeactivate()
    {
        lightCount = 0;
        ActivateBlock(false);
    }

    // -----------------------------
    // 内部クラス: Trigger を Forward するだけ
    // -----------------------------
    private class TriggerForwarder : MonoBehaviour
    {
        [HideInInspector] public LightSensitiveBlock targetBlock;
        [HideInInspector] public LayerMask detectableLayers;

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & detectableLayers) == 0) return;

            if (other.GetComponent<LightRange>() != null)
                targetBlock?.AddLight();
        }

        private void OnTriggerExit(Collider other)
        {
            if (((1 << other.gameObject.layer) & detectableLayers) == 0) return;

            if (other.GetComponent<LightRange>() != null)
                targetBlock?.RemoveLight();
        }
    }
}
