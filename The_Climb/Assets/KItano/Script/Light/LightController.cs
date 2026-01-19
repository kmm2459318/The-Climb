using UnityEngine;
using System;

public class LightController : MonoBehaviour
{
    [Header("ライト本体")]
    [SerializeField] private Light targetLight;

    [Header("ライトの色")]
    [SerializeField] private Color whiteColor = Color.white;
    [SerializeField] private Color purpleColor = new Color(0.5f, 0f, 1f);

    [Header("照射設定")]
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private int rayCount = 7;        // 円錐Ray本数
    [SerializeField] private float coneAngle = 40f;   // 扇形角度（度）

    private bool isPurple = false;

    // イベント
    public static event Action<GameObject, Color> OnLightEnter;
    public static event Action<GameObject, Color> OnLightExit;
    public static event Action<Color> OnLightColorChanged;

    // 前フレームで照らしていたオブジェクト
    private GameObject currentHitObject = null;

    void Start()
    {
        if (targetLight != null)
            targetLight.color = whiteColor;
    }

    void Update()
    {
        HandleColorSwitch();
        HandleDirection2D();
        CheckConeRaycast2D();
    }

    // -------------------------------------------------
    // 色切り替え（Eキー）
    // -------------------------------------------------
    private void HandleColorSwitch()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isPurple = !isPurple;
            Color newColor = isPurple ? purpleColor : whiteColor;
            targetLight.color = newColor;

            OnLightColorChanged?.Invoke(newColor);
            Debug.Log($"LightController：Color switched to {(isPurple ? "Purple" : "White")}");
        }
    }

    // -------------------------------------------------
    // ライトの向き（X-Y平面固定）
    // -------------------------------------------------
    private void HandleDirection2D()
    {
        Vector3 mousePos = Input.mousePosition;

        // ライトと同じZ平面でワールド座標取得
        mousePos.z = Mathf.Abs(
            Camera.main.transform.position.z - targetLight.transform.position.z
        );

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        Vector3 dir = worldPos - targetLight.transform.position;
        dir.z = 0f;            // ★ Z方向を完全に無視
        dir.Normalize();

        targetLight.transform.forward = dir;
    }

    // -------------------------------------------------
    // 円錐Raycast（2D横スクロール用）
    // -------------------------------------------------
    private void CheckConeRaycast2D()
    {
        GameObject hitThisFrame = null;

        for (int i = 0; i < rayCount; i++)
        {
            float t = (float)i / (rayCount - 1) - 0.5f; // -0.5 ～ 0.5
            float angle = t * coneAngle;

            // ★ Z軸回転で扇状にRayを広げる
            Vector3 dir =
                Quaternion.Euler(0f, 0f, angle) * targetLight.transform.forward;

            if (Physics.Raycast(
                targetLight.transform.position,
                dir,
                out RaycastHit hit,
                maxDistance))
            {
                Debug.DrawRay(
                    targetLight.transform.position,
                    dir * hit.distance,
                    Color.green
                );

                LightReveal reveal =
                    hit.collider.GetComponentInParent<LightReveal>();

                if (reveal != null)
                {
                    hitThisFrame = reveal.gameObject;
                    break; // 1つ見つかればOK
                }
            }
            else
            {
                Debug.DrawRay(
                    targetLight.transform.position,
                    dir * maxDistance,
                    Color.gray
                );
            }
        }

        // 照射対象が変わったらイベント発火
        if (hitThisFrame != currentHitObject)
        {
            if (currentHitObject != null)
            {
                OnLightExit?.Invoke(currentHitObject, targetLight.color);
                Debug.Log($"LightController：Light Exit {currentHitObject.name}");
            }

            if (hitThisFrame != null)
            {
                OnLightEnter?.Invoke(hitThisFrame, targetLight.color);
                Debug.Log($"LightController：Light Enter {hitThisFrame.name}");
            }

            currentHitObject = hitThisFrame;
        }
    }
}
