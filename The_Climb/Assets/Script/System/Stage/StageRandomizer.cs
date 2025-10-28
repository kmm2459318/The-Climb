using UnityEngine;
using UnityEngine.SceneManagement;

public class StageRandomizer : MonoBehaviour
{
    public string[] StageName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShuffleStage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ステージをランダムにする処理
    private void ShuffleStage()
    {
        string StageHold;
        for(int i = 0; i < StageName.Length; i++)
        {
            StageHold = StageName[i]; // i番目のステージを保存
            int  StageNo = Random.Range(0 + i, StageName.Length); // 決まったステージを除くステージの中からランダムで選ばれる
            StageName[i] = StageName[StageNo]; // i番目のステージを変更
            StageName[StageNo] = StageHold; // Holdに入ったステージをランダムで選ばれた場所に代入

            Debug.Log($"ステージ{i}番目は{StageName[i]}に決まりました");
        }
    }

    public void StartStage(int ButtonNo)
    {
        SceneManager.LoadScene(StageName[ButtonNo]);
    }
}
