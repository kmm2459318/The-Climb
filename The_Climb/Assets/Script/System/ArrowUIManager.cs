using UnityEngine;

public class ArrowUIManager : MonoBehaviour
{
    RectTransform arrowRectTransform = null;

    [SerializeField] private Transform target;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform arrowRect;

    private float rateScale = 4.5f;
    private float scaleMin = 0.5f;
    private float scaleMax = 1.5f;

    [SerializeField] private Vector2 edgeBuffer = new Vector2(50f, 50f);

    void Start()
    {
        arrowRectTransform = GetComponent<RectTransform>();

        arrowRectTransform.localScale = Vector3.one;
    }

    void Update()
    {
        Vector3 screenPos = uiCamera.WorldToScreenPoint(target.position);

        bool isOnScreen =
        screenPos.z > 0 &&
        screenPos.x >= 0 && screenPos.x <= Screen.width &&
        screenPos.y >= 0 && screenPos.y <= Screen.height;

        // 画面内なら非表示
        if (isOnScreen)
        {
            arrowRect.gameObject.SetActive(false);
            return;
        }

        // 画面外なら表示
        arrowRect.gameObject.SetActive(true);

        //Vector3 toTarget = target.position - uiCamera.transform.position;
        //float distance = toTarget.magnitude;
        //float scale = Mathf.Clamp(rateScale / distance, scaleMin, scaleMax);

        // 画面端クランプ
        screenPos.x = Mathf.Clamp(screenPos.x, edgeBuffer.x, Screen.width - edgeBuffer.x);
        screenPos.y = Mathf.Clamp(screenPos.y, edgeBuffer.y, Screen.height - edgeBuffer.y);

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            null,
            out localPos
        );

        arrowRect.localPosition = localPos;
    }
}
