using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public string[] StageName;
    public void GameStart()
    {
        PlayerPrefs.SetInt("GameStart", 1); // ステージをシャッフルするかどうか
        PlayerPrefs.SetInt("SelectStage", 0); // 選択できるステージをリセット

        PlayerPrefs.SetString("VisitedStages", null); // クリアしたステージの情報をリセット
        PlayerPrefs.Save();

        for(int i= 0; StageName.Length>i; i++ )
        {
            PlayerPrefs.SetInt($"{StageName[i]}", 0); // クリアしたステージの情報をリセット
        }
        
        SceneManager.LoadScene("StageSelect");
    }
}
