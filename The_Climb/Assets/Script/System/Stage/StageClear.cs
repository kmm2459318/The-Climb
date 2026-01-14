using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClear : MonoBehaviour
{
    private bool isClearing = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isClearing) return;

        if(other.tag == "Player")
        {
            isClearing = true;
            string StageName = SceneManager.GetActiveScene().name; // クリアしたステージの名前を取得
            Debug.Log($"ステージ{StageName}クリア!!");
            PlayerPrefs.SetInt($"{StageName}", 1); // クリアした情報を保存
            System.Loading.SceneLoader.Instance.LoadScene("StageSelect"); // ステージセレクトに戻す
        }
    }
}
