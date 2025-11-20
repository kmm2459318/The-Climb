using UnityEngine;
using System.Collections;

public class SkillPlatformSpawner : MonoBehaviour
{
    [Header("参照設定")]
    public Transform player;               // ← プレイヤーをInspectorで指定
    public GameObject platformPrefab;      // 足場のPrefab
    public GameObject triggerColliderPrefab; // コライダーPrefab（IsTrigger = true）

    [Header("スキル設定")]
    public KeyCode activateKey = KeyCode.E;
    public float skillCooldown = 3f;       // 使用間隔（秒）
    public float colliderShrinkDuration = 1.5f; // コライダーが消えるまでの時間

    [Header("生成位置設定")]
    public Vector3 spawnOffset = new Vector3(0, -0.49f, 0); // プレイヤーの足元に生成
    public float zShiftOnEnd = -2f;        // 消滅時にずらす距離（Z軸方向）

    private bool canUseSkill = true;

    void Update()
    {
        if (player == null) return;

        if (Input.GetKeyDown(activateKey) && canUseSkill && PlayerPrefs.GetInt("UmedaAbi") == 1)
        {
            StartCoroutine(SpawnPlatform());
        }
    }

    private IEnumerator SpawnPlatform()
    {
        canUseSkill = false;

        // プレイヤーの足元の座標を取得
        Vector3 spawnPos = player.position + spawnOffset;

        // 足場とトリガーを生成
        GameObject platform = Instantiate(platformPrefab, spawnPos, Quaternion.identity);
        GameObject trigger = Instantiate(triggerColliderPrefab, spawnPos, Quaternion.identity);

        // トリガーの縮小アニメーション
        float timer = 0f;
        Vector3 initialScale = trigger.transform.localScale;

        while (timer < colliderShrinkDuration)
        {
            timer += Time.deltaTime;
            float t = timer / colliderShrinkDuration;
            trigger.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            yield return null;
        }

        // コライダーが完全に縮んだ瞬間
        // → 足場とトリガーをZ軸方向にずらす
        platform.transform.position += new Vector3(0, 0, zShiftOnEnd);
        trigger.transform.position += new Vector3(0, 0, zShiftOnEnd);

        // 少し待ってから破壊（0.1秒で自然な消え方に）
        yield return new WaitForSeconds(0.1f);

        platform.SetActive(false);
        trigger.SetActive(false);

        // クールタイム
        yield return new WaitForSeconds(skillCooldown);
        canUseSkill = true;
    }
}
