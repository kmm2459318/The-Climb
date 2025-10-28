using UnityEngine;

public class OrbitalMotion : MonoBehaviour
{
    [Header("ライトの移動用スクリプト(円)")]
    [Header("回転の中心")]
    public Transform center; // 回転の中心になるオブジェクト

    [Header("回転設定")]
    public float radius = 2f;       // 回転半径
    public float speed = 30f;       // 回転スピード（度/秒）
    public bool clockwise = true;   // 時計回りか反時計回りか

    private float angle; // 現在の角度（度）

    void Start()
    {
        if (center == null)
        {
            // 中心が設定されていない場合は親を中心にする
            center = transform.parent;
        }

        // 初期位置を半径に応じてセット
        if (center != null)
        {
            Vector3 offset = new Vector3(radius, 0, 0);
            transform.position = center.position + offset;
        }
    }

    void Update()
    {
        if (center == null) return;

        // 回転方向
        float direction = clockwise ? -1f : 1f;

        // 角度更新
        angle += speed * Time.deltaTime * direction;
        if (angle >= 360f || angle <= -360f) angle = 0f;

        // 新しい位置を計算
        float rad = angle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;
        transform.position = center.position + offset;
    }
}