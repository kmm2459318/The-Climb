using UnityEngine;

public class DeathZone : MonoBehaviour
{
    [Header("落下で死亡させるプレイヤー")]
    public GameObject[] players;   // Player1, Player2 をInspectorで登録

    [Header("ゲームオーバーUI（Canvasなど）")]
    public GameObject gameOverUI;  // InspectorでCanvasを登録

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("DeathZoneに入った: " + other.name);

        // プレイヤー配列の中にいるか判定
        foreach (GameObject player in players)
        {
            if (other.gameObject == player 
             || other.transform.root.gameObject == player) // 子オブジェクトにも対応
            {
                // プレイヤーを消す
                player.SetActive(false);

                // ゲームオーバーUIを表示
                if (gameOverUI != null)
                {
                    gameOverUI.SetActive(true);
                }

                Debug.Log(player.name + " が奈落に落下 → ゲームオーバー！");
            }
        }
    }
}
