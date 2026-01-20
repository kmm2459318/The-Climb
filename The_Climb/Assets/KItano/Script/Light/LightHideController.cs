using UnityEngine;
using System;

public class LightHideController : MonoBehaviour
{
    public event Action<GameObject, Color> OnLightEnter;

    [Header("光の設定")]
    [SerializeField] private float range = 10f;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private Color lightColor = Color.white;

    private GameObject lastHit;

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitLayer))
        {
            if (hit.collider.gameObject != lastHit)
            {
                lastHit = hit.collider.gameObject;
                Debug.Log($"LightHideController({name})：Hit {lastHit.name}");
                OnLightEnter?.Invoke(lastHit, lightColor);
            }
        }
        else
        {
            lastHit = null;
        }
    }
}
