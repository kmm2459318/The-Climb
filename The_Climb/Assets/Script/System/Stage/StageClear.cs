using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClear : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            string StageName = SceneManager.GetActiveScene().name; // クリアしたステージの名前を取得
            Debug.Log($"ステージ{StageName}クリア!!");
            PlayerPrefs.SetInt($"{StageName}", 1); // クリアした情報を保存
            SceneManager.LoadScene("StageSelect"); // ステージセレクトに戻す
        }
    }
}
