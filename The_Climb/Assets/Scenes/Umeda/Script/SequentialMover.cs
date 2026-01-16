using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialMover : MonoBehaviour
{
    [Header("ライトの移動用スクリプト(直線)")]

    [Header("移動ポイント")]
    public List<Transform> waypoints = new List<Transform>();
    public float speed = 3f;
    public float stopDuration = 0f;

    [Header("スイッチ（任意）")]
    public Switch startSwitch;   // ★ 追加：スイッチ参照

    private int currentIndex = 0;
    private bool isWaiting = false;

    void Update()
    {
        // スイッチが設定されていて、まだ押されていないなら動かない
        if (startSwitch != null && !startSwitch.IsPressed)
            return;

        if (waypoints.Count == 0 || isWaiting) return;

        Transform target = waypoints[currentIndex];
        if (target == null) return;

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            StartCoroutine(WaitAndMoveNext());
        }
    }

    private IEnumerator WaitAndMoveNext()
    {
        isWaiting = true;

        if (stopDuration > 0f)
            yield return new WaitForSeconds(stopDuration);

        currentIndex = (currentIndex + 1) % waypoints.Count;
        isWaiting = false;
    }
}
