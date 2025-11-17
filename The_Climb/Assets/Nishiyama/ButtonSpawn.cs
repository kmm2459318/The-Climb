using UnityEngine;
using System.Collections;

public class ButtonGimmick : MonoBehaviour
{
    [Header("押せるボタンのTransform")]
    [SerializeField] private Transform buttonTransform; // 見た目のボタン
    [SerializeField] private float pressDepth = 0.1f;   // 押し込み量
    [SerializeField] private float pressSpeed = 5f;     // アニメ速度

    [Header("出現させたいオブジェクト")]
    [SerializeField] private GameObject targetObject;

    [Header("オブジェクト出現時間")]
    [SerializeField] private float activeTime = 3f;

    private bool isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤータグを持つものが触れたら起動
        if (other.CompareTag("Player") && !isActivated)
        {
            StartCoroutine(ActivateGimmick());
            if (buttonTransform != null)
                StartCoroutine(PressButtonAnimation());
        }
    }

    private IEnumerator ActivateGimmick()
    {
        isActivated = true;

        // ターゲットを出現
        if (targetObject != null)
            targetObject.SetActive(true);

        // 出現時間待機
        yield return new WaitForSeconds(activeTime);

        // ターゲットを非表示
        if (targetObject != null)
            targetObject.SetActive(false);

        isActivated = false;
    }

    private IEnumerator PressButtonAnimation()
    {
        if (buttonTransform == null) yield break;

        Vector3 originalPos = buttonTransform.localPosition;
        Vector3 pressedPos = originalPos - new Vector3(0, pressDepth, 0);

        // 下に押し込む
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * pressSpeed;
            buttonTransform.localPosition = Vector3.Lerp(originalPos, pressedPos, t);
            yield return null;
        }

        // 少し待つ
        yield return new WaitForSeconds(2f);

        // 元に戻す
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * pressSpeed;
            buttonTransform.localPosition = Vector3.Lerp(pressedPos, originalPos, t);
            yield return null;
        }
    }
}
