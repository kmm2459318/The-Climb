using UnityEngine;

public class LightRaycaster : MonoBehaviour
{
    public float rayDistance = 10f;
    public LayerMask targetLayer;

    void Update()
    {
        Vector3 direction = transform.right; // ← XY平面方向

        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, targetLayer))
        {
            RaycastReactiveObject reactive =
                hit.collider.GetComponent<RaycastReactiveObject>();

            if (reactive != null)
            {
                reactive.OnRaycastHit();
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            transform.position,
            transform.position + transform.right * rayDistance
        );
    }
}
