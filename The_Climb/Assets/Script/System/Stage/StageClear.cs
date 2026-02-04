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

        // StageRandomizer / StageSelectManager 共通で使っているID
        int currentStageId = PlayerPrefs.GetInt("CurrentStageId", -1);

        if (currentStageId == -1)
        {
            Debug.LogError("StageClear: CurrentStageId が取得できません");
        }
        else
        {
            Debug.Log($"ステージ {currentStageId} をクリア");

            // ★ ゴール時のみ「本当にクリアしたステージ」を渡す
            PlayerPrefs.SetInt("JustClearedStageId", currentStageId);

            // （任意）シーン単位のクリアフラグが必要なら残す
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 1);

            PlayerPrefs.Save();
        }

        // ステージセレクトへ戻る
        System.Loading.SceneLoader.Instance.LoadScene("StageSelect");
    }
}
