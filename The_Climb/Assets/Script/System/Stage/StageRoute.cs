using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageRoute : MonoBehaviour
{
    // 各ボタンをインスペクターで登録（1〜8の順）
    public List<StageNode> stageButtons;

    // 通過済みステージを保存
    private HashSet<int> visitedStages = new HashSet<int>();

    // ステージ分岐辞書
    private Dictionary<int, List<int>> routes = new Dictionary<int, List<int>>()
    {
        { 0, new List<int>{ 1, 2 } }, // 最初の2択を表示
        { 1, new List<int>{ 3, 4 } }, // てんたい → 相棒・アイテム切り替え
        { 2, new List<int>{ 4, 5 } }, // 時間切り替え → アイテム切り替え・操作めちゃくちゃ
        { 3, new List<int>{ 6 } },    // 相棒 → なんか
        { 4, new List<int>{ 6, 7 } }, // アイテム切り替え → なんか・足場消えちゃう
        { 5, new List<int>{ 7 } },    // 操作めちゃくちゃ → 足場消えちゃう
        { 6, new List<int>{ 8 } },    // なんか → ゴール
        { 7, new List<int>{ 8 } },    // 足場消えちゃう → ゴール
        { 8, new List<int>() }        // ゴール → 終点
    };

    void Start()
    {
        // 直前に選ばれたステージ番号を取得（初回は0）
        int lastStage = PlayerPrefs.GetInt("SelectStage", 0);

        // 通過済みステージの情報を読み込み
        LoadVisitedStages();

        // 最初に有効化するボタンを設定
        SetActiveButtons(routes[lastStage]);
    }

    public void OnStageButtonPressed(int buttonNumber)
    {
        Debug.Log("選択されたボタン番号：" + buttonNumber);

        // 通過済みとして登録
        visitedStages.Add(buttonNumber);
        SaveVisitedStages();

        // 次に進むステージを決定
        int nextStage = buttonNumber;
        PlayerPrefs.SetInt("SelectStage", nextStage);

        // 次の選択肢を表示
        if (routes.ContainsKey(nextStage))
        {
            SetActiveButtons(routes[nextStage]);
        }
        else
        {
            // 終点
            SetActiveButtons(new List<int>());
        }
    }

    // ボタンの有効/無効と色を切り替える
    void SetActiveButtons(List<int> activeNumbers)
    {
        for (int i = 0; i < stageButtons.Count; i++)
        {
            int buttonIndex = i + 1;
            StageNode btn = stageButtons[i];
            Image btnImage = btn.GetComponent<Image>();

            if (visitedStages.Contains(buttonIndex))
            {
                // 通過済み → 無効化＋黄色
                btn.enabled = false;
                //btnImage.color = Color.yellow;
            }
            else if (activeNumbers.Contains(buttonIndex))
            {
                // 現在選択可能 → 有効化＋白
                btn.enabled = true;
                //btnImage.color = Color.white;
            }
            else
            {
                // それ以外 → 無効化＋灰色
                btn.enabled = false;
                //btnImage.color = Color.gray;
            }
        }
    }

    // 通過済みステージを保存
    void SaveVisitedStages()
    {
        string savedData = string.Join(",", visitedStages);
        PlayerPrefs.SetString("VisitedStages", savedData);
        PlayerPrefs.Save();
    }

    // 通過済みステージを読み込み
    void LoadVisitedStages()
    {
        string savedData = PlayerPrefs.GetString("VisitedStages", "");
        if (!string.IsNullOrEmpty(savedData))
        {
            string[] parts = savedData.Split(',');
            foreach (string part in parts)
            {
                if (int.TryParse(part, out int num))
                    visitedStages.Add(num);
            }
        }
    }
}
