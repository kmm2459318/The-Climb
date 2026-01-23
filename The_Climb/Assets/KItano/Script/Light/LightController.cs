//using UnityEngine;
//using System;

//public class LightController : MonoBehaviour
//{
//    // ① 光が当たったことを通知するイベント
//    public static event Action<GameObject, Color> OnLightEnter;

//    [Header("光線設定")]
//    [SerializeField] private float lightDistance = 10f;     // 光の届く距離
//    [SerializeField] private float lightRadius = 0.3f;      // 光の太さ
//    [SerializeField] private LayerMask lightHitLayer;        // 光判定専用レイヤー

//    [Header("光の色")]
//    [SerializeField] private Color lightColor = Color.white;

//    [Header("デバッグ")]
//    [SerializeField] private bool showDebugRay = true;

//    void Update()
//    {
//        EmitLight();
//    }

//    private void EmitLight()
//    {
//        Ray ray = new Ray(transform.position, transform.forward);
//        RaycastHit hit;

//        // ② SphereCastで光線判定（距離調整可能）
//        bool isHit = Physics.SphereCast(
//            ray,
//            lightRadius,
//            out hit,
//            lightDistance,
//            lightHitLayer
//        );

//        // ③ デバッグ表示
//        if (showDebugRay)
//        {
//            Color debugColor = isHit ? Color.yellow : Color.blue;
//            Debug.DrawRay(
//                transform.position,
//                transform.forward * lightDistance,
//                debugColor
//            );
//        }

//        if (!isHit) return;

//        // ④ ヒットしたオブジェクト取得
//        GameObject hitObj = hit.collider.gameObject;

//        Debug.Log($"① 光ヒット検出：{hitObj.name}");

//        // ⑤ LightRevealを持っているか確認
//        LightReveal reveal = hitObj.GetComponent<LightReveal>();
//        if (reveal == null)
//        {
//            Debug.Log($"② {hitObj.name} に LightReveal がありません");
//            return;
//        }

//        // ⑥ 光ヒットイベント発火
//        Debug.Log($"③ OnLightEnter 発火：{hitObj.name}");
//        OnLightEnter?.Invoke(hitObj, lightColor);
//    }

//    // Sceneビューで視覚的に距離を確認
//    private void OnDrawGizmosSelected()
//    {
//        Gizmos.color = Color.cyan;
//        Gizmos.DrawLine(
//            transform.position,
//            transform.position + transform.forward * lightDistance
//        );
//    }
//}
