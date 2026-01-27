using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

namespace Hanzzz.MeshDemolisher
{
    public class MeshDemolisherExample : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private GameObject targetGameObject;
        [SerializeField] private Transform breakPointsParent;
        [SerializeField] private Material interiorMaterial;

        [Header("Result Pieces")]
        [SerializeField] private Transform resultParent;
        [SerializeField, Range(0f, 1f)] private float resultScale = 0.9f;
        [SerializeField] private float shrinkDuration = 1.0f;
        [SerializeField] private float fallTimeBeforeShrink = 1.5f; // ★ 落下猶予

        [Header("Performance")]
        [SerializeField] private int maxFallingPieces = 8;
        [SerializeField] private int processPerFrame = 3;

        [Header("UI")]
        [SerializeField] private TMP_Text logText;

        private static MeshDemolisher meshDemolisher = new MeshDemolisher();

        private bool requestDemolish;
        private bool isDemolished;

        // =============================
        // プレイヤーが乗ったかの判定or爆弾があったかの判定
        // =============================
        public void RequestDemolish()
        {
            if (isDemolished) return;
            requestDemolish = true;
        }

        private void Update()
        {
            if (!requestDemolish) return;

            requestDemolish = false;
            isDemolished = true;
            StartCoroutine(DemolishFlow());
        }

        // =============================
        // 破壊フロー（完成形）
        // =============================
        private IEnumerator DemolishFlow()
        {
            // 既存破片削除
            foreach (Transform c in resultParent)
                Destroy(c.gameObject);

            // breakPoint数制限
            List<Transform> breakPoints = new List<Transform>();
            int count = Mathf.Min(maxFallingPieces, breakPointsParent.childCount);

            for (int i = 0; i < count; i++)
                breakPoints.Add(breakPointsParent.GetChild(i));

            yield return null;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            List<GameObject> pieces =
                meshDemolisher.Demolish(targetGameObject, breakPoints, interiorMaterial);
            watch.Stop();

            if (logText != null)
                logText.text = $"Demolish time: {watch.ElapsedMilliseconds} ms";

            // 元オブジェクト非表示
            targetGameObject.SetActive(false);

            // ★ breakPointsParent の Collider を OFF
            DisableBreakPointColliders();

            // 破片配置
            foreach (GameObject p in pieces)
            {
                p.transform.SetParent(resultParent, true);
                p.transform.localScale = resultScale * Vector3.one;
            }

            // 重力付与
            yield return StartCoroutine(ActivateGravityOnly());

            // ★ 落ちる時間を与える
            yield return new WaitForSeconds(fallTimeBeforeShrink);

            // 縮小して消す
            yield return StartCoroutine(ShrinkPieces());
        }

        // =============================
        // 重力のみ付与
        // =============================
        private IEnumerator ActivateGravityOnly()
        {
            int processed = 0;

            foreach (Transform piece in resultParent)
            {
                if (!piece.TryGetComponent(out Collider _))
                    piece.gameObject.AddComponent<BoxCollider>();

                Rigidbody rb = piece.gameObject.AddComponent<Rigidbody>();
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.WakeUp(); 

                processed++;
                if (processed >= processPerFrame)
                {
                    processed = 0;
                    yield return null;
                }
            }
        }

        // =============================
        // 縮小して消す
        // =============================
        private IEnumerator ShrinkPieces()
        {
            float t = 0f;

            while (t < shrinkDuration)
            {
                float scale = Mathf.Lerp(resultScale, 0f, t / shrinkDuration);

                foreach (Transform c in resultParent)
                    if (c != null)
                        c.localScale = scale * Vector3.one;

                t += Time.deltaTime;
                yield return null;
            }

            foreach (Transform c in resultParent)
                if (c != null)
                    Destroy(c.gameObject);
        }

        // =============================
        // breakPoint の Collider を OFF
        // =============================
        private void DisableBreakPointColliders()
        {
            foreach (Collider col in breakPointsParent.GetComponentsInChildren<Collider>())
            {
                col.enabled = false;
            }
        }

        [ContextMenu("Demolish")]
        public void Demolish()
        {
            Enumerable.Range(0, resultParent.childCount).Select(i => resultParent.GetChild(i)).ToList().ForEach(x => DestroyImmediate(x.gameObject)); List<Transform> breakPoints = Enumerable.Range(0, breakPointsParent.childCount).Select(x => breakPointsParent.GetChild(x)).ToList();
            var watch = System.Diagnostics.Stopwatch.StartNew(); List<GameObject> res = meshDemolisher.Demolish(targetGameObject, breakPoints, interiorMaterial); 
            watch.Stop(); logText.text = $"Demolish time: {watch.ElapsedMilliseconds}ms."; res.ForEach(x => x.transform.SetParent(resultParent, true));
            Enumerable.Range(0, resultParent.childCount).Select(i => resultParent.GetChild(i)).ToList().ForEach(x => x.localScale = resultScale * Vector3.one);
            targetGameObject.SetActive(false); 
        }

        [ContextMenu("Demolish Async")] 
        public async void DemolishAsync() 
        {
            Enumerable.Range(0, resultParent.childCount).Select(i => resultParent.GetChild(i)).ToList().ForEach(x => DestroyImmediate(x.gameObject));
            List<Transform> breakPoints = Enumerable.Range(0, breakPointsParent.childCount).Select(x => breakPointsParent.GetChild(x)).ToList();
            var watch = System.Diagnostics.Stopwatch.StartNew(); List<GameObject> res = await meshDemolisher.DemolishAsync(targetGameObject, breakPoints, interiorMaterial); 
            watch.Stop(); logText.text = $"Demolish time: {watch.ElapsedMilliseconds}ms."; res.ForEach(x => x.transform.SetParent(resultParent, true));
            Enumerable.Range(0, resultParent.childCount).Select(i => resultParent.GetChild(i)).ToList().ForEach(x => x.localScale = resultScale * Vector3.one); 
            targetGameObject.SetActive(false);
        }


        // =============================
        // リセット
        // =============================
        [ContextMenu("Reset")]
        public void Reset()
        {
            isDemolished = false;
            requestDemolish = false;

            foreach (Transform c in resultParent)
                DestroyImmediate(c.gameObject);

            // breakPoint Collider を戻す
            foreach (Collider col in breakPointsParent.GetComponentsInChildren<Collider>())
                col.enabled = true;

            targetGameObject.SetActive(true);
        }
    }
}