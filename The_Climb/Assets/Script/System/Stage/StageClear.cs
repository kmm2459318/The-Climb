using UnityEngine;

public class StageClear : MonoBehaviour
{
    private bool cleared = false;

    private void OnTriggerEnter(Collider other)
    {
        if (cleared) return;
        if (!other.CompareTag("Player")) return;

        cleared = true;

        int stageId = PlayerPrefs.GetInt("CurrentStageId", -1);
        if (stageId >= 0)
        {
            PlayerPrefs.SetInt("LastClearedStage", stageId);
            PlayerPrefs.Save();
        }

        System.Loading.SceneLoader.Instance.LoadScene("StageSelect");
    }
}
