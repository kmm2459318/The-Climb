using UnityEngine;

/// <summary>
/// 光や特定のトリガーに反応して、指定ブロック群の表示・当たり判定を切り替える。
/// GameObject自体は無効化せず、RendererとColliderだけを切り替える軽量版。
/// </summary>
[DisallowMultipleComponent]
public class TriggerBlockActivator : MonoBehaviour
{
    [Header("表示・消失させるターゲット（複数可）")]
    public GameObject[] targetBlocks;

    [Header("反応させるレイヤー")]
    public LayerMask detectableLayers;

    private void Start()
    {
        // ✅ 初期状態：すべて非表示・当たり判定オフ
        if (targetBlocks != null)
        {
            foreach (var block in targetBlocks)
            {
                if (block == null) continue;
                SetBlockVisible(block, false);
            }
        }

        // ✅ Triggerが機能するようにRigidbodyを確認
        Collider col = GetComponent<Collider>();
        if (col != null && col.isTrigger && GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & detectableLayers) == 0) return;

        // ✅ 有効化
        SetBlocksVisible(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & detectableLayers) == 0) return;

        // ✅ 無効化
        SetBlocksVisible(false);
    }

    // -----------------------------------------
    // 個々のブロックの表示・判定を切り替え
    // -----------------------------------------
    private void SetBlocksVisible(bool visible)
    {
        if (targetBlocks == null) return;

        foreach (var block in targetBlocks)
        {
            if (block == null) continue;

            // 🟡 無効化前に上のRigidbodyを起こす
            if (!visible)
            {
                WakeUpObjectsAbove(block);
            }

            SetBlockVisible(block, visible);
        }
    }

    // -----------------------------------------
    // RendererとColliderを切り替える
    // -----------------------------------------
    private void SetBlockVisible(GameObject block, bool visible)
    {
        // RendererをOFF/ON
        Renderer renderer = block.GetComponent<Renderer>();
        if (renderer != null)
            renderer.enabled = visible;

        // ColliderをOFF/ON
        Collider collider = block.GetComponent<Collider>();
        if (collider != null)
            collider.enabled = visible;
    }

    // -----------------------------------------
    // ブロックが消えるとき、上のRigidbodyを起こす
    // -----------------------------------------
    private void WakeUpObjectsAbove(GameObject block)
    {
        Collider[] hits = Physics.OverlapBox(
            block.transform.position + Vector3.up * 0.5f,
            new Vector3(0.5f, 0.5f, 0.5f),
            Quaternion.identity
        );

        foreach (var hit in hits)
        {
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
            {
                rb.WakeUp();
                rb.AddForce(Vector3.down * 0.01f, ForceMode.VelocityChange);
            }
        }
    }
}
