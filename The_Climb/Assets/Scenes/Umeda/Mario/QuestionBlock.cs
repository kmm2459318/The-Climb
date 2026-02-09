using UnityEngine;
using System.Collections;

public class QuestionBlock : MonoBehaviour
{
    [Header("ループ表示モデル（1〜4）")]
    public GameObject[] loopModels; // サイズ4
    public GameObject usedModel;

    [Header("アニメ設定")]
    public float loopInterval = 0.15f;
    public float bumpHeight = 0.2f;
    public float bumpDuration = 0.1f;

    bool isUsed = false;
    int currentIndex = 0;
    float loopTimer;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;

        ShowOnly(loopModels[0]);
        usedModel.SetActive(false);
    }

    void Update()
    {
        if (isUsed) return;

        loopTimer += Time.deltaTime;
        if (loopTimer >= loopInterval)
        {
            loopTimer = 0f;
            currentIndex = (currentIndex + 1) % loopModels.Length;
            ShowOnly(loopModels[currentIndex]);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isUsed) return;

        if (!collision.gameObject.CompareTag("Player")) return;

        // 下から叩かれたか判定
        ContactPoint contact = collision.contacts[0];
        if (contact.normal.y > 0.5f)
        {
            StartCoroutine(Bump());
        }
    }

    IEnumerator Bump()
    {
        isUsed = true;

        // 上に少し動かす
        yield return MoveBlock(startPos + Vector3.up * bumpHeight);

        // 元に戻す
        yield return MoveBlock(startPos);

        // 使用済み表示
        ShowOnly(usedModel);
    }

    IEnumerator MoveBlock(Vector3 target)
    {
        Vector3 from = transform.localPosition;
        float t = 0f;

        while (t < bumpDuration)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(from, target, t / bumpDuration);
            yield return null;
        }

        transform.localPosition = target;
    }

    void ShowOnly(GameObject target)
    {
        foreach (var obj in loopModels)
            obj.SetActive(false);

        usedModel.SetActive(false);
        target.SetActive(true);
    }
}
