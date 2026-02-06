#if UNITY_EDITOR
using UnityEditor.Overlays;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public string[] StageName;
    public void GameStart()
    {
        PlayerPrefs.SetInt("GameStart", 1); // ステージをシャッフル・リセットするかどうかのフラグ
        PlayerPrefs.SetInt("SelectStage", 0); // 選択位置をリセット

        // 古いクリア情報のキーを個別にリセット
        PlayerPrefs.DeleteKey("VisitedStages"); 
        PlayerPrefs.DeleteKey("ClearedStages"); // StageSelectManagerで使っているキーも削除しておく
        
        for(int i= 0; StageName.Length>i; i++ )
        {
            PlayerPrefs.SetInt($"{StageName[i]}", 0); // 各シーン名ごとのクリア情報をリセット
        }
        
        PlayerPrefs.Save();
        SceneManager.LoadScene("StageSelect"); // ステージ選択シーンへ遷移
    }

    public void EndGame()
    {
        SceneManager.LoadScene("Title");
    }
}
