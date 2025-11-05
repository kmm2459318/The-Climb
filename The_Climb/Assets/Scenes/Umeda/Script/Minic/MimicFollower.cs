using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MimicFollower : MonoBehaviour
{
    [Header("模倣設定")]
    public float delay = 1.0f; // 遅延時間（秒）
    public float pushForceUp = 10f;
    public float pushForceForward = 5f;

    [Header("プレイヤー制御")]
    public float disableControlTime = 0.8f; // 操作不能時間（秒）

    private Rigidbody rb;
    private Animator animator;
    private int frameDelay;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.isKinematic = false; // 物理的に落ちないようにしたいなら true にしてもOK
    }

    private void Start()
    {
        // 秒 → フレーム数換算
        frameDelay = Mathf.RoundToInt(delay / Time.fixedDeltaTime);
    }

    private void FixedUpdate()
    {
        var recorder = PlayerMimicRecorder.Instance;
        if (recorder == null) return;

        if (recorder.HistoryCount <= frameDelay) return;

        if (recorder.TryGetHistory(frameDelay, out var pos, out var rot, out var anim))
        {
            rb.MovePosition(pos);
            rb.MoveRotation(rot);
            animator.Play(anim.shortNameHash, 0, anim.normalizedTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // プレイヤーを吹っ飛ばす
                Vector3 pushDir = (other.transform.position - transform.position).normalized;
                playerRb.AddForce(Vector3.up * pushForceUp + pushDir * pushForceForward, ForceMode.Impulse);
            }

            // 一瞬操作不能にする
            StartCoroutine(TemporarilyDisablePlayerControl(other.gameObject));
        }
    }

    private IEnumerator TemporarilyDisablePlayerControl(GameObject player)
    {
        // PlayerMove または PlayerController を取得して一時停止
        var move = player.GetComponent<PlayerMove>();
        var controller = player.GetComponent<PlayerController>();

        if (move != null)
            move.enabled = false;
        if (controller != null)
            controller.enabled = false;

        yield return new WaitForSeconds(disableControlTime);

        if (move != null)
            move.enabled = true;
        if (controller != null)
            controller.enabled = true;
    }
}
