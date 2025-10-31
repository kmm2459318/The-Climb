using UnityEngine;
using UnityEngine.SceneManagement;  // シーンをリロードするために必要

public class DeathZone : MonoBehaviour
{
    [Header("落下で死亡させるプレイヤー")]
    public GameObject[] players;   // Player1, Player2 をInspectorで登録

    [Header("ゲームオーバーUI（Canvasなど）")]
    public GameObject gameOverUI;  // InspectorでCanvasを登録

    private bool isGameOver = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isGameOver) return; // 二重処理防止

        Debug.Log("DeathZoneに入った: " + other.name);

        foreach (GameObject player in players)
        {
            if (other.gameObject == player || other.transform.root.gameObject == player)
            {
                player.SetActive(false);

                if (gameOverUI != null)
                {
                    gameOverUI.SetActive(true);
                }

                isGameOver = true;
                Debug.Log(player.name + " が奈落に落下 → ゲームオーバー！");
            }
        }
    }

    // UIのボタンにこのメソッドを登録すればリスタート可能
    public void RestartGame()
    {
        Debug.Log("シーンをリスタートします");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
