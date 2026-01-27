// ----------------------------
// PlayerFlashLight.cs（統合版）
// ----------------------------
using UnityEngine;

public class PlayerFlashLight : MonoBehaviour
{
    [Header("ライト関連")]
    [SerializeField] private LightRevealController controller;
    [SerializeField] private GameObject flashLightVisual;

    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;
        Debug.Log("[③] PlayerFlashLight 初期化完了");
    }

    private void Update()
    {
        HandleFlashLight();
        AimToMouse();
    }

    // ----------------------------
    // フラッシュライトON/OFF管理
    // ----------------------------
    private void HandleFlashLight()
    {
        if (Input.GetMouseButtonDown(1))
        {
            flashLightVisual.SetActive(true);
            controller.SetFlashLight(true);
            Debug.Log("[③] フラッシュライトON");
        }

        if (Input.GetMouseButtonUp(1))
        {
            flashLightVisual.SetActive(false);
            controller.SetFlashLight(false);
            Debug.Log("[③] フラッシュライトOFF");
        }
    }

    // ----------------------------
    // マウス方向に回転
    // ----------------------------
    private void AimToMouse()
    {
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z; // 2Dの場合はZ固定

        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
