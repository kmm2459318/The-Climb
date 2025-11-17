using UnityEngine;

public class CameraFollowHorizontalTwoPlayers : MonoBehaviour
{
    [SerializeField] private Transform playerTop;      // 上のプレイヤー
    [SerializeField] private Transform playerBottom;   // 下のプレイヤー
    [SerializeField] private float smoothSpeed = 5f;   // カメラ追従スピード
    [SerializeField] private Vector3 offset = new Vector3(0, 8.5f, -10f); // 固定オフセット位置

    private float fixedY;
    private float fixedZ;

    void Start()
    {
        // 最初の高さと奥行きを固定
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (playerTop == null || playerBottom == null)
        {
            Debug.LogWarning("プレイヤーの参照が設定されていません！");
            return;
        }

        // 2人の中間点を求める
        Vector3 center = (playerTop.position + playerBottom.position) / 2f;

        // Xだけ追従、YとZは固定
        Vector3 targetPos = new Vector3(center.x + offset.x, fixedY + offset.y, fixedZ + offset.z);

        // スムーズに移動
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}
