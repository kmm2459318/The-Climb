using UnityEngine;
using System;

public class Switch : MonoBehaviour
{
    // ★ 全Switch共通イベント
    public static event Action<Switch> OnSwitchPressed;

    [Header("識別ID（Activatorと一致させる）")]
    public string switchID;

    [Header("下がる見た目（孫オブジェクト）")]
    public Transform visualObject;

    [Header("下がるコライダー（孫オブジェクト）")]
    public Transform pressDownCollider;

    [Header("スイッチテキスト")]
    public GameObject switchText;

    [Header("押下量")]
    public float pressDownDistance = 2.0f;

    [Header("押下後に削除")]
    public bool destroyOnPressed = false;

    private Vector3 visualInitialLocalPos;
    private Vector3 colliderInitialLocalPos;
    private bool isPressed = false;

    public bool IsPressed => isPressed;

    void Start()
    {
        if (visualObject != null)
            visualInitialLocalPos = visualObject.localPosition;

        if (pressDownCollider != null)
            colliderInitialLocalPos = pressDownCollider.localPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPressed) return;
        if (!other.CompareTag("Player")) return;
        if (other.transform.position.y < transform.position.y) return;

        Press();
    }

    void Press()
    {
        isPressed = true;

        if (visualObject != null)
            visualObject.localPosition =
                visualInitialLocalPos + Vector3.down * pressDownDistance;

        if (pressDownCollider != null)
            pressDownCollider.localPosition =
                colliderInitialLocalPos + Vector3.down * pressDownDistance;

        if (switchText != null)
            switchText.SetActive(false);

        // ★ 通知（Destroy前に必ず呼ぶ）
        OnSwitchPressed?.Invoke(this);

        if (destroyOnPressed)
        {
            Destroy(gameObject);
        }
    }

    public void ForceReset()
    {
        isPressed = false;

        if (visualObject != null)
            visualObject.localPosition = visualInitialLocalPos;

        if (pressDownCollider != null)
            pressDownCollider.localPosition = colliderInitialLocalPos;

        if (switchText != null)
            switchText.SetActive(true);
    }
}
