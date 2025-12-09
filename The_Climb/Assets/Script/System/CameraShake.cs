using ModestTree;
using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Vector3 ShakeOffset = Vector3.zero;
    private Coroutine ShakeCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    private void LateUpdate()
    {
        transform.position += ShakeOffset;
    }

    public void Shake(float Duration = 0.2f, float Magnitude = 0.3f)
    {
        if (ShakeCoroutine != null)
        {
            StopCoroutine(ShakeCoroutine);
        }

        ShakeCoroutine = StartCoroutine(ShakeRoutine(Duration, Magnitude));
    }

    private IEnumerator ShakeRoutine(float Duration, float Magnitude)
    {
        float Elapsed = 0f;

        while (Elapsed < Duration)
        {
            float X = Random.Range(-1f, 1f) * Magnitude;
            float Y = Random.Range(-1f, 1f) * Magnitude;

            ShakeOffset = new Vector3(X, Y, 0);

            Elapsed += Time.deltaTime;
            yield return null;
        }

        ShakeOffset = Vector3.zero;
    }
}