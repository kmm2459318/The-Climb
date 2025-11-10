using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialMover : MonoBehaviour
{
    [Header("ライトの移動用スクリプト(直線)")]
    [Header("移動ポイント")]
    public List<Transform> waypoints = new List<Transform>(); // 移動先のオブジェクト
    public float speed = 3f; // 移動速度
    public float stopDuration = 0f; // 各地点で停止する時間（秒）。0なら止まらない

    private int currentIndex = 0; // 現在のターゲット
    private bool isWaiting = false; // 停止中フラグ

    void Update()
    {
        if (waypoints.Count == 0 || isWaiting) return;

        Transform target = waypoints[currentIndex];
        if (target == null) return;

        // 現在位置からターゲット方向への移動
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // ターゲットに到達したかチェック（誤差を考慮）
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            // 次の地点へ進む前に停止処理を挟む
            StartCoroutine(WaitAndMoveNext());
        }
    }

    private IEnumerator WaitAndMoveNext()
    {
        isWaiting = true;

        // 停止時間が設定されている場合は待機
        if (stopDuration > 0f)
            yield return new WaitForSeconds(stopDuration);

        // 次のターゲットに移動（ループ）
        currentIndex = (currentIndex + 1) % waypoints.Count;
        isWaiting = false;
    }
}