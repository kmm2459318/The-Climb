using UnityEngine;
using UnityEngine.SceneManagement;

public class StageClear : MonoBehaviour
{
    private bool isClearing = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isClearing) return;
        if (!other.CompareTag("Player")) return;

        isClearing = true;

        // 現在のステージID（StageRandomizerでセット済み）
        int currentStageId = PlayerPrefs.GetInt("CurrentStageId", 0);
        int lastClearedStage = PlayerPrefs.GetInt("LastClearedStage", 0);

        string key = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetInt(key, 1);

        Debug.Log($"{key} の値 = " + PlayerPrefs.GetInt(key));

        Debug.Log($"ステージ {currentStageId} クリア");

        // ★ 最大値のみ更新（戻り防止）
        if (currentStageId > lastClearedStage)
        {
            PlayerPrefs.SetInt("LastClearedStage", currentStageId);
            PlayerPrefs.Save();
        }

        // ステージセレクトへ戻る
        System.Loading.SceneLoader.Instance.LoadScene("StageSelect");
    }
}
