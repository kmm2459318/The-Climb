using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class StartGame : MonoBehaviour
{
    [FormerlySerializedAs("ScenePool")]
    [FormerlySerializedAs("StageName")]
    public string[] SceneName; // リセット対象の全シーン名

    public TextMeshProUGUI UITextMeshPro;

    public void Start()
    {
        int ClearStagecount = PlayerPrefs.GetInt("ClearStagecount", 0);
        UITextMeshPro.text = ($"読破数 {ClearStagecount}/6"); 
    }
    public void GameStart()
    {
        PlayerPrefs.SetInt("GameStart", 1); 
        PlayerPrefs.SetInt("SelectStage", 0); 

        PlayerPrefs.DeleteKey("VisitedStages"); 
        PlayerPrefs.DeleteKey("ClearedStages"); 
        
        // シーン名ベースのフラグリセット
        if (SceneName != null)
        {
            foreach (string scene in SceneName)
            {
                PlayerPrefs.SetInt(scene, 0);
            }
        }

        // ステージIDベースのフラグリセット (0〜20)
        for (int i = 0; i < 20; i++)
        {
            PlayerPrefs.SetInt($"StageCleared_{i}", 0);
        }
        PlayerPrefs.DeleteKey("JustClearedStageId");
        
        PlayerPrefs.Save();
        SceneManager.LoadScene("StageSelect"); 
    }
    public void EndGame()
    {
        SceneManager.LoadScene("Title");
    }
}
