using UnityEngine;
using System.Collections.Generic;

public class StageRandomizer : MonoBehaviour
{
    public string[] StageName;
    public string[] BossStageName;

    private void Start()
    {
        if (PlayerPrefs.GetInt("GameStart") == 1)
        {
            Shuffle();
            Save();
            PlayerPrefs.SetInt("GameStart", 0);
        }
        else
        {
            Load();
        }
    }

    private void Shuffle()
    {
        List<string> src = new List<string>(StageName);
        for (int i = 0; i < src.Count; i++)
        {
            int r = Random.Range(i, src.Count);
            (src[i], src[r]) = (src[r], src[i]);
        }
        StageName = src.ToArray();
    }

    private void Save()
    {
        PlayerPrefs.SetString("StageOrder", string.Join(",", StageName));
    }

    private void Load()
    {
        if (!PlayerPrefs.HasKey("StageOrder")) return;
        StageName = PlayerPrefs.GetString("StageOrder").Split(',');
    }

    public void StartStage(int stageId)
    {
        PlayerPrefs.SetInt("CurrentStageId", stageId);
        PlayerPrefs.Save();

        System.Loading.SceneLoader.Instance.LoadScene(StageName[stageId]);
    }
}
