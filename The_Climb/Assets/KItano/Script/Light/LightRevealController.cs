// ----------------------------
// LightRevealController.cs
// ----------------------------
using UnityEngine;
using System;

public class LightRevealController : MonoBehaviour
{
    public event Action<GameObject, Color> OnLightEnter;

    [Header("フラッシュライト設定")]
    [SerializeField] private float range = 10f;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private Color lightColor = Color.white;

    private GameObject lastHit;
    private bool isFlashLightOn = false;

    // ----------------------------
    // フラッシュライトON/OFF
    // ----------------------------
    public void SetFlashLight(bool on)
    {
        if (!on)
        {
            lastHit = null; // 再照射可能にする
            Debug.Log("[①] FlashLight OFF → lastHitリセット");
        }
        else
        {
            Debug.Log("[①] FlashLight ON");
        }

        isFlashLightOn = on;
    }

    // ----------------------------
    // LightRevealから呼ぶ用：lastHitをリセット
    // ----------------------------
    public void ResetLastHit()
    {
        lastHit = null;
        Debug.Log("[①] ResetLastHit呼び出し → 再照射可能");
    }

    private void Update()
    {
        if (!isFlashLightOn) return;

        Ray ray = new Ray(transform.position, transform.forward);

        // SceneビューでRayを可視化（デバッグ用）
        Debug.DrawRay(transform.position, transform.forward * range, Color.yellow);

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitLayer))
        {
            if (hit.collider.gameObject != lastHit)
            {
                lastHit = hit.collider.gameObject;
                Debug.Log($"[①] Hit {lastHit.name}");
                OnLightEnter?.Invoke(lastHit, lightColor);
            }
        }
    }

    // Gizmosで常時SceneビューにRayを表示
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, transform.forward * range);
    }
}
