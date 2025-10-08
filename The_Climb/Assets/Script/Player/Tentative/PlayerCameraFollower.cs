using UnityEngine;

public class PlayerCameraFollower : MonoBehaviour
{
    [Header("追従させるカメラ")]
    public Camera targetCamera;

    [Header("各軸の追従を有効にするか")]
    public bool followX = true;
    public bool followY = true;
    public bool followZ = true;

    private Vector3 initialOffset;

    private void Start()
    {
        if (targetCamera == null)
        {
            Debug.LogWarning("[PlayerCameraFollower] カメラが指定されていません！");
            return;
        }

        // 初期オフセット（カメラ - プレイヤーの差）を保存
        initialOffset = targetCamera.transform.position - transform.position;
    }

    private void LateUpdate()
    {
        if (targetCamera == null) return;

        Vector3 targetPosition = targetCamera.transform.position;

        // プレイヤーの位置にオフセットを足す（各軸ごとに条件分岐）
        Vector3 desiredPosition = targetPosition;

        if (followX)
            desiredPosition.x = transform.position.x + initialOffset.x;

        if (followY)
            desiredPosition.y = transform.position.y + initialOffset.y;

        if (followZ)
            desiredPosition.z = transform.position.z + initialOffset.z;

        targetCamera.transform.position = desiredPosition;
    }
}
