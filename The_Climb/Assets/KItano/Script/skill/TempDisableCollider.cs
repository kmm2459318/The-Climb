using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TempDisableColliders : MonoBehaviour
{
    [SerializeField] private List<Collider> colliders = new List<Collider>(); // 無効化したいコライダーたち
    [SerializeField] private float duration = 2f; // 無効化しておく時間
    [SerializeField] private float cooldownTime = 3f; // クールタイム(秒)
    private float currentCooldownTimer = 0f; // クールタイムタイマー
    private bool isRunning = false;

    void Update()
    {
        // isRunning(効果中またはクールタイム中)なら発動できない
        if (Input.GetKeyDown(KeyCode.Q) && !isRunning && PlayerPrefs.GetInt("KitanoAbi") == 1)
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

        // 指定時間待つ(効果時間)
        yield return new WaitForSeconds(duration);

        // 全コライダーを再び有効化
        foreach (var col in colliders)
        {
            if (col != null)
                col.enabled = true;
        }
        
        // クールタイム処理
        currentCooldownTimer = 0f;
        while (currentCooldownTimer < cooldownTime)
        {
            currentCooldownTimer += Time.deltaTime;
            yield return null;
        }

        isRunning = false;
        currentCooldownTimer = 0f;
    }

    /// <summary>
    /// クールタイム進行状況を取得(0.0~1.0、1.0が満タン)
    /// </summary>
    public float GetCooldownProgress()
    {
        if (!isRunning)
        {
            return 0f; // 待機状態
        }

        if (currentCooldownTimer > 0f)
        {
            float progress = 1f - (currentCooldownTimer / cooldownTime);
            return Mathf.Clamp01(progress);
        }

        return 0f;
    }
}
