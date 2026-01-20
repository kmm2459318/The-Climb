using UnityEngine;
using System;

public class LightRevealController : MonoBehaviour
{
    public event Action<GameObject, Color> OnLightEnter;

    [Header("光設定")]
    [SerializeField] private float range = 10f;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private Color lightColor = Color.white;

    private GameObject lastHit;

    private void Awake()
    {
        Debug.Log($"LightRevealController：Awake ({gameObject.name})");
    }

    private void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitLayer))
        {
            if (hit.collider.gameObject != lastHit)
            {
                lastHit = hit.collider.gameObject;
                Debug.Log($"LightRevealController：Hit {lastHit.name}");
                OnLightEnter?.Invoke(lastHit, lightColor);
            }
        }
        else
        {
            lastHit = null;
        }
    }
}
