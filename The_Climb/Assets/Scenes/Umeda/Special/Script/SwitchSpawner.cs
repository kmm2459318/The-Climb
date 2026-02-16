using UnityEngine;

public class SwitchSpawner : MonoBehaviour
{
    [Header("対象スイッチ")]
    public Switch targetSwitch;

    [Header("生成するPrefab")]
    public GameObject spawnPrefab;

    [Header("生成数")]
    public int spawnCount = 10;

    [Header("生成範囲")]
    public Vector3 spawnAreaSize = new Vector3(5f, 0f, 5f);

    [Header("生成基準位置（未設定ならこのオブジェクト）")]
    public Transform spawnOrigin;

    [Header("生成位置を0固定にするか")]
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;

    [Header("一度だけ実行")]
    public bool spawnOnlyOnce = true;

    private bool executed = false;

    void Update()
    {
        if (spawnOnlyOnce && executed) return;
        if (targetSwitch == null || spawnPrefab == null) return;

        if (targetSwitch.IsPressed)
        {
            SpawnObjects();
            executed = true;
        }
    }

    void SpawnObjects()
    {
        Transform origin = spawnOrigin != null ? spawnOrigin : transform;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 randomOffset = new Vector3(
                lockX ? 0f : Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
                lockY ? 0f : Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f),
                lockZ ? 0f : Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f)
            );

            Vector3 spawnPos = origin.position + randomOffset;

            // ★ 親を指定して生成
            Instantiate(spawnPrefab, spawnPos, Quaternion.identity, origin);
        }
    }
}
