using UnityEngine;

public class HorizontalMirrorMove : MonoBehaviour
{
    [SerializeField] private Transform player1; // 上のプレイヤー
    [SerializeField] private Transform player2; // 下のプレイヤー
    [SerializeField] private float moveSpeed = 5f;

    void Update()
    {
        float input = Input.GetAxisRaw("Horizontal"); // A/Dキー or ←→キー

        if (input != 0)
        {
            // 現在の中間点を計算（横方向でも上下でもOK）
            Vector3 centerPoint = (player1.position + player2.position) / 2f;

            // Player1を入力方向に動かす
            player1.position += Vector3.right * input * moveSpeed * Time.deltaTime;

            // Player2を中心点に対してX軸方向で反転させる
            Vector3 offset = player1.position - centerPoint;
            offset.x *= -1; // ← Xだけ反転
            player2.position = centerPoint + offset;
        }
    }
}
