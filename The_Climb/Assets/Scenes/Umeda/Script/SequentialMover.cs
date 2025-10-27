using System.Collections.Generic;
using UnityEngine;

public class SequentialMover : MonoBehaviour
{
    [Header("ライトの移動用スクリプト(直線)")]
    [Header("移動ポイント")]
    public List<Transform> waypoints = new List<Transform>(); // 移動先のオブジェクト
    public float speed = 3f; // 移動速度

    private int currentIndex = 0; // 現在のターゲット

    void Update()
    {
        if (waypoints.Count == 0) return;

        Transform target = waypoints[currentIndex];
        if (target == null) return;

        // 現在位置からターゲット方向への移動
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // ターゲットに到達したかチェック（誤差を考慮）
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentIndex = (currentIndex + 1) % waypoints.Count; // 次のターゲットに移動、ループ
        }
    }
}