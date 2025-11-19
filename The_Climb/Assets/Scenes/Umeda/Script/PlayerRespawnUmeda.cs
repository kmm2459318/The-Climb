using UnityEngine;
using System.Collections.Generic;

public class PlayerRespawnUmeda : MonoBehaviour
{
    private Rigidbody rb;

    [Header("リスポーンポイントリスト")]
    public List<Transform> respawnPoints = new List<Transform>(); // 手動設定用リスト

    [Header("現在のリスポーンインデックス")]
    [SerializeField] private int currentIndex = 0;

    private Vector3 lastSavePos;

    // チェックポイントの見た目制御用（同じ順番で登録）
    [Header("対応するチェックポイント見た目リスト")]
    public List<CheckpointVisual> checkpointVisuals = new List<CheckpointVisual>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (respawnPoints.Count > 0)
        {
            lastSavePos = respawnPoints[0].position;
            UpdateCheckpointVisual(0);
        }
        else
        {
            lastSavePos = transform.position;
            Debug.LogWarning("⚠️ リスポーンポイントが未設定です。現在位置を初期値にします。");
        }
    }

    void Update()
    {
        if (transform.position.y < -4.3f)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.MovePosition(lastSavePos);
        }
        else
        {
            transform.position = lastSavePos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Checkpointタグに触れたら該当Indexを探す
        for (int i = 0; i < respawnPoints.Count; i++)
        {
            if (respawnPoints[i] != null && other.transform == respawnPoints[i])
            {
                SetRespawnPoint(i);
                break;
            }
        }
    }

    public void SetRespawnPoint(int index)
    {
        if (index >= 0 && index < respawnPoints.Count)
        {
            currentIndex = index;
            lastSavePos = respawnPoints[index].position;
            UpdateCheckpointVisual(index);
            Debug.Log($"✅ リスポーン地点を更新しました → {respawnPoints[index].name}");
        }
    }

    void UpdateCheckpointVisual(int activeIndex)
    {
        for (int i = 0; i < checkpointVisuals.Count; i++)
        {
            if (checkpointVisuals[i] != null)
                checkpointVisuals[i].SetActiveState(i == activeIndex);
        }
    }

    public Transform GetCurrentRespawnPoint()
    {
        if (respawnPoints.Count == 0) return null;
        return respawnPoints[currentIndex];
    }
}
