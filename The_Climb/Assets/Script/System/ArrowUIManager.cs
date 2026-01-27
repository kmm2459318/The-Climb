using UnityEngine;
using System.Collections.Generic;


public class ArrowUIManager : MonoBehaviour
{
    RectTransform arrowRectTransform = null;

    private Transform target;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform arrowRect;

    private float rateScale = 4.5f;
    private float scaleMin = 0.5f;
    private float scaleMax = 1.5f;

    [SerializeField] private Vector2 edgeBuffer = new Vector2(50f, 50f);

    [SerializeField] private List<Transform> buttons;

    void Start()
    {
        arrowRectTransform = GetComponent<RectTransform>();

        arrowRectTransform.localScale = Vector3.one;
        target = buttons[0].transform;
    }

    void Update()
    {
        //ターゲット設定
        SetTarget();
        Vector3 screenPos = uiCamera.WorldToScreenPoint(target.position);

        bool isOnScreen =
        screenPos.z > 0 &&
        screenPos.x >= 0 && screenPos.x <= Screen.width &&
        screenPos.y >= 0 && screenPos.y <= Screen.height;

        //画面内なら非表示
        if (isOnScreen)
        {
            arrowRect.gameObject.SetActive(false);
            ScreenInArrow(screenPos);
        }
        else
        {
            ScreenOutArrow(screenPos);
        }
    }

    //各ボタンの距離を調べ、ターゲットを設定
    void SetTarget()
    {
        float minDistance = float.MaxValue;
        foreach (Transform btn in buttons)
        {
            Vector3 toTarget = btn.position - uiCamera.transform.position;
            float distance = toTarget.magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                target = btn;
            }
        }
    }

    //画面内矢印表示
    void ScreenInArrow(Vector3 pos)
    {
        //画面外なら表示
        arrowRect.gameObject.SetActive(true);

        //Vector3 toTarget = target.position - uiCamera.transform.position;
        //float distance = toTarget.magnitude;
        //float scale = Mathf.Clamp(rateScale / distance, scaleMin, scaleMax);

        arrowRect.rotation = Quaternion.Euler(0f, 0f, 0f);

        pos.y += 190f;  //少し上にずらす

        //位置変換
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            pos,
            null,
            out localPos
        );

        arrowRect.localPosition = localPos;
    }

    //画面外矢印表示
    void ScreenOutArrow(Vector3 pos)
    {
        //画面外なら表示
        arrowRect.gameObject.SetActive(true);

        //Vector3 toTarget = target.position - uiCamera.transform.position;
        //float distance = toTarget.magnitude;
        //float scale = Mathf.Clamp(rateScale / distance, scaleMin, scaleMax);

        //画面端クランプ
        Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0) * 0.5f;
        Vector3 fromCenterToTarget = (pos - screenCenter).normalized;

        pos = screenCenter + fromCenterToTarget * ((Mathf.Min(Screen.width, Screen.height) * 0.5f) - edgeBuffer.magnitude);
        pos.x = Mathf.Clamp(pos.x, edgeBuffer.x, Screen.width - edgeBuffer.x);
        pos.y = Mathf.Clamp(pos.y, edgeBuffer.y, Screen.height - edgeBuffer.y);

        //角度計算と回転
        float angle = Mathf.Atan2(
            pos.y - 2.5f - (Screen.height / 2),
            pos.x - (Screen.width / 2)
        ) * Mathf.Rad2Deg;
        arrowRect.rotation = Quaternion.Euler(0, 0, angle + 90f);

        //位置変換
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            pos,
            null,
            out localPos
        );

        arrowRect.localPosition = localPos;
    }
}
