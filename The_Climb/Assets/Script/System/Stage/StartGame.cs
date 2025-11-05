using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public void GameStart()
    {
        PlayerPrefs.SetInt("GameStart", 1); // ステージをシャッフルするかどうか
        PlayerPrefs.SetInt("SelectStage", 0); // 選択できるステージをリセット

        PlayerPrefs.SetString("VisitedStages", null); // クリアしたステージの情報をリセット
        PlayerPrefs.Save();

        SceneManager.LoadScene("StageSelect");
    }
}
