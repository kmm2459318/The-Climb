using UnityEngine;
using System.Collections;

public class LightImpactCollider : MonoBehaviour
{
    [Header("床オブジェクト設定（ColliderB）")]
    public Transform floorVisual;          // 床の見た目（透明Sphereなど）

    [Header("スケール設定")]
    public float minScale = 0.0f;          // 消えた状態の大きさ
    public float maxScale = 1.0f;          // 光が当たっている時の大きさ
    public float expandTime = 0.3f;        // 拡大時間
    public float shrinkSpeed = 0.5f;       // 縮小スピード

    [Header("検知レイヤー設定")]
    public LayerMask lightLayer;           // LuminaLightBall のレイヤー

    private Coroutine scaleRoutine;
    private bool isExpanding = false;
    private int lightCount = 0; // 同時に複数の光が当たった場合も対応

    private void Start()
    {
        if (floorVisual != null)
            floorVisual.localScale = Vector3.one * minScale;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & lightLayer.value) == 0) return;

        lightCount++;
        if (lightCount == 1) // 初めて光が当たった
        {
            if (scaleRoutine != null) StopCoroutine(scaleRoutine);
            scaleRoutine = StartCoroutine(ExpandRoutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & lightLayer.value) == 0) return;

        lightCount = Mathf.Max(0, lightCount - 1);
        if (lightCount == 0) // 全ての光が離れた
        {
            if (scaleRoutine != null) StopCoroutine(scaleRoutine);
            scaleRoutine = StartCoroutine(ShrinkRoutine());
        }
    }

    private IEnumerator ExpandRoutine()
    {
        isExpanding = true;
        float timer = 0f;
        Vector3 start = floorVisual.localScale;
        Vector3 target = Vector3.one * maxScale;

        while (timer < expandTime)
        {
            timer += Time.deltaTime;
            float t = timer / expandTime;
            floorVisual.localScale = Vector3.Lerp(start, target, t);
            yield return null;
        }

        floorVisual.localScale = target;
        isExpanding = false;
    }

    private IEnumerator ShrinkRoutine()
    {
        isExpanding = false;
        float current = floorVisual.localScale.x;

        while (current > minScale)
        {
            current = Mathf.MoveTowards(current, minScale, Time.deltaTime * shrinkSpeed);
            floorVisual.localScale = Vector3.one * current;
            yield return null;
        }

        floorVisual.localScale = Vector3.one * minScale;
    }

    public void CreateImpact(Vector3 position)
    {
        // ここにコライダー生成・拡大処理を書く
        Debug.Log("CreateImpact called at " + position);
    }
}
