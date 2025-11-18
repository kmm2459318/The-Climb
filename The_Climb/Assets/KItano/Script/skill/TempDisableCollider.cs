using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TempDisableColliders : MonoBehaviour
{
    [SerializeField] private List<Collider> colliders = new List<Collider>(); // 無効化したいコライダーたち
    [SerializeField] private float duration = 2f; // 無効化しておく時間

    private bool isRunning = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isRunning)
        {
            StartCoroutine(DisableRoutine());
        }
    }

    private IEnumerator DisableRoutine()
    {
        isRunning = true;

        // 全コライダーを無効化
        foreach (var col in colliders)
        {
            if (col != null)
                col.enabled = false;
        }

        // 指定時間待つ
        yield return new WaitForSeconds(duration);

        // 全コライダーを再び有効化
        foreach (var col in colliders)
        {
            if (col != null)
                col.enabled = true;
        }

        isRunning = false;
    }
}
