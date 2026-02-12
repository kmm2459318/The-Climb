using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ReenableObjectsOnLoad : MonoBehaviour
{
    [Header("シーン読み込み時に一度無効化して再有効化するオブジェクト")]
    public List<GameObject> targetObjects = new List<GameObject>();

    void Awake()
    {
        // シーン読み込み時のコールバックを登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // 念のため解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ReenableCoroutine());
    }

    private System.Collections.IEnumerator ReenableCoroutine()
    {
        foreach (var obj in targetObjects)
        {
            if (obj == null) continue;

            // 一度無効化
            obj.SetActive(false);
        }

        // 1フレーム待機
        yield return null;

        foreach (var obj in targetObjects)
        {
            if (obj == null) continue;

            // 再度有効化
            obj.SetActive(true);
        }
    }
}
