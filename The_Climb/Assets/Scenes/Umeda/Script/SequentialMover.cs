using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialMover : MonoBehaviour
{
    [Header("移動ポイント")]
    public List<Transform> waypoints = new List<Transform>();
    public float speed = 3f;
    public float stopDuration = 0f;

    [Header("スイッチ（任意）")]
    public Switch startSwitch;

    private int currentIndex = 0;
    private bool isWaiting = false;

    // ★ 追加
    private bool hasStarted = false;
    private bool lastSwitchState = false;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;

        if (startSwitch != null)
            lastSwitchState = startSwitch.IsPressed;
    }

    void Update()
    {
        // スイッチ未設定 → 常時動作
        if (startSwitch == null)
        {
            Move();
            return;
        }

        bool currentSwitchState = startSwitch.IsPressed;

        // ▼ スイッチが押された瞬間
        if (!lastSwitchState && currentSwitchState)
        {
            // 初回 or 再押下 → 最初から
            ResetMover();
            hasStarted = true;
        }

        lastSwitchState = currentSwitchState;

        // ★ まだ一度も起動していないなら動かない
        if (!hasStarted) return;

        Move();
    }

    void Move()
    {
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

    void ResetMover()
    {
        StopAllCoroutines();
        isWaiting = false;
        currentIndex = 0;
        transform.position = startPosition;
    }
}
