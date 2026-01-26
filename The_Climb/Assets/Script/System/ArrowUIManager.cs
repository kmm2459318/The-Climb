using UnityEngine;

public class ArrowUIManager : MonoBehaviour
{
    RectTransform arrowRectTransform = null;

    public Transform target;
    public Camera uiCamera;

    void Start()
    {
        arrowRectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);

        arrowRectTransform.position = uiCamera.WorldToScreenPoint(target.position);
    }
}
