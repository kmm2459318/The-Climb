using UnityEngine;

public class LightRaycaster : MonoBehaviour
{
    public float rayDistance = 10f;
    public LayerMask targetLayer;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
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
            transform.position + transform.forward * rayDistance
        );
    }
}
