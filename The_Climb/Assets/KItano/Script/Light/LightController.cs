using UnityEngine;
using System;

public class LightController : MonoBehaviour
{
    [Header("ライト本体")]
    [SerializeField] private Light targetLight;

    [Header("ライトの色設定")]
    [SerializeField] private Color whiteColor = Color.white;
    [SerializeField] private Color purpleColor = new Color(0.5f, 0f, 1f);

    [Header("照射設定")]
    [SerializeField] private float maxDistance = 20f;

    // 現在照射している相手
    private GameObject currentHitObject = null;

    // イベント（他のスクリプトが登録する）
    public static event Action<GameObject, Color> OnLightEnter;
    public static event Action<GameObject, Color> OnLightExit;
    public static event Action<Color> OnLightColorChanged;

    private bool isPurple = false;

    void Start()
    {
        if (targetLight != null)
            targetLight.color = whiteColor;
    }

    void Update()
    {
        HandleColorSwitch();
        HandleDirection();
        CheckRaycastHit();
    }

    // -------------------------
    // ① Eキーで色を切り替え
    // -------------------------
    private void HandleColorSwitch()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            isPurple = !isPurple;
            Color newColor = isPurple ? purpleColor : whiteColor;

            targetLight.color = newColor;

            // 他スクリプトへ通知
            OnLightColorChanged?.Invoke(newColor);
        }
    }

    // -------------------------
    // ② マウス方向にライトを向ける
    // -------------------------
    private void HandleDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // カメラ距離調整

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 dir = (worldPos - targetLight.transform.position).normalized;

        targetLight.transform.forward = dir;
    }

    // -------------------------
    // ③ 何を照らしているか毎フレーム確認
    // -------------------------
    private void CheckRaycastHit()
    {
        Ray ray = new Ray(targetLight.transform.position, targetLight.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (currentHitObject != hitObj)
            {
                // 前のオブジェクトへの照射終了を通知
                if (currentHitObject != null)
                    OnLightExit?.Invoke(currentHitObject, targetLight.color);

                // 新しいオブジェクトへ照射開始を通知
                OnLightEnter?.Invoke(hitObj, targetLight.color);

                currentHitObject = hitObj;
            }
        }
        else
        {
            // 今照らしてない → 前のオブジェクトにExitを通知
            if (currentHitObject != null)
                OnLightExit?.Invoke(currentHitObject, targetLight.color);

            currentHitObject = null;
        }
    }
}
