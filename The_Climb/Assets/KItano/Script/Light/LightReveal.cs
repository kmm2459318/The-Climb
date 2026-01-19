using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Renderer))]
public class LightReveal : MonoBehaviour
{
    [Header("出現設定")]
    [SerializeField] private float revealTime = 2f;

    [Header("Collider設定")]
    [SerializeField] private Collider objectCollider;

    private Renderer rend;
    private bool isRevealed = false;
    private Coroutine revealRoutine;

    void Awake()
    {
        rend = GetComponent<Renderer>();

        Color c = rend.material.color;
        c.a = 0f;
        rend.material.color = c;

        if (objectCollider) objectCollider.enabled = false;
    }

    void OnEnable()
    {
        LightController.OnLightEnter += HandleLightEnter;
    }

    void OnDisable()
    {
        LightController.OnLightEnter -= HandleLightEnter;
    }

    private void HandleLightEnter(GameObject hitObj, Color color)
    {
        if (isRevealed || hitObj != this.gameObject) return;

        if (revealRoutine != null) StopCoroutine(revealRoutine);
        revealRoutine = StartCoroutine(RevealCoroutine());
    }

    private IEnumerator RevealCoroutine()
    {
        float elapsed = 0f;
        Color startColor = rend.material.color;
        Color targetColor = startColor;
        targetColor.a = 1f;

        while (elapsed < revealTime)
        {
            elapsed += Time.deltaTime;
            rend.material.color = Color.Lerp(startColor, targetColor, elapsed / revealTime);
            yield return null;
        }

        rend.material.color = targetColor;

        if (objectCollider) objectCollider.enabled = true;

        isRevealed = true;

        Debug.Log($"LightReveal：{gameObject.name} 完全出現、Collider有効化");
    }
}
