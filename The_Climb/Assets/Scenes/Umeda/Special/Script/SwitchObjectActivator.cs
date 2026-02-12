using UnityEngine;

public class SwitchObjectActivator : MonoBehaviour
{
    [Header("反応するSwitchのID")]
    public string targetSwitchID;

    [Header("スポーンするプレハブ")]
    public GameObject spawnPrefab;

    [Header("スポーン位置")]
    public Transform spawnPoint;

    [Header("デスポーンするオブジェクト")]
    public GameObject despawnTarget;

    [Header("一度だけ実行")]
    public bool executeOnce = true;

    private bool executed;

    void OnEnable()
    {
        Switch.OnSwitchPressed += OnSwitchPressed;
    }

    void OnDisable()
    {
        Switch.OnSwitchPressed -= OnSwitchPressed;
    }

    void OnSwitchPressed(Switch sw)
    {
        if (executeOnce && executed) return;

        // ★ ID一致判定
        if (sw.switchID != targetSwitchID) return;

        Execute();
    }

    void Execute()
    {
        executed = true;

        // スポーン or 再アクティブ
        if (spawnPrefab != null)
        {
            Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
            Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

            Instantiate(spawnPrefab, pos, rot);
        }

        // デスポーン
        if (despawnTarget != null)
        {
            Destroy(despawnTarget);
        }
    }
}
