using UnityEngine;

[DisallowMultipleComponent]
public class TriggerBlockActivator : MonoBehaviour
{
    [Header("表示・消失させるターゲット（複数可）")]
    public GameObject[] targetBlocks;

    [Header("反応させるレイヤー")]
    public LayerMask detectableLayers;

    private void Start()
    {
        // 最初は全て無効化
        if (targetBlocks != null)
        {
            foreach (var block in targetBlocks)
            {
                if (block != null)
                    block.SetActive(false);
            }
        }

        // Triggerが機能するようにRigidbodyを確認
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

        // 有効化
        SetBlocksActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & detectableLayers) == 0) return;

        // 無効化
        SetBlocksActive(false);
    }

    private void SetBlocksActive(bool active)
    {
        if (targetBlocks == null) return;

        foreach (var block in targetBlocks)
        {
            if (block == null) continue;

            // 🟡 無効化直前に、上のRigidbodyを起こす
            if (!active)
            {
                WakeUpObjectsAbove(block);
            }

            block.SetActive(active);
        }
    }

    private void WakeUpObjectsAbove(GameObject block)
    {
        // 足場の上の物体（Rigidbody）を起こす
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
                rb.WakeUp(); // ✅ Rigidbodyを強制的に再アクティブ化
                rb.AddForce(Vector3.down * 0.01f, ForceMode.VelocityChange); // 少しだけ下に刺激
            }
        }
    }
}
