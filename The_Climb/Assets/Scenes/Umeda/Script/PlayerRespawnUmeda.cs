using UnityEngine;
using System.Collections.Generic;

public class PlayerRespawnUmeda : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerMove playerMove;

    [Header("リスポーンポイントリスト")]
    public List<Transform> respawnPoints = new List<Transform>(); // 手動設定用リスト

    [Header("現在のリスポーンインデックス")]
    [SerializeField] private int currentIndex = 0;

    private Transform currentRespawnPoint;

    // チェックポイントの見た目制御用（同じ順番で登録）
    [Header("対応するチェックポイント見た目リスト")]
    public List<CheckpointVisual> checkpointVisuals = new List<CheckpointVisual>();

    [Header("リスポーン判定設定（チェックポイントからの相対距離）")]
    public float maxHeightFromCheckpoint = 30f; // 上方向の制限（これ以上離れるとリスポーン）
    public float fallDistanceFromCheckpoint = 20f; // 下方向の制限（これ以上落ちるとリスポーン）

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMove = GetComponent<PlayerMove>();

        if (respawnPoints.Count > 0)
        {
            SetRespawnPoint(0);
        }
        else
        {
            Debug.LogWarning("⚠️ リスポーンポイントが未設定です。");
        }
    }

    void Update()
    {
        if (currentRespawnPoint == null) return;

        // 現在のチェックポイント（currentRespawnPoint）とのY座標差分を計算
        float diffY = transform.position.y - currentRespawnPoint.position.y;

        // 上に行き過ぎた場合 OR 下に落ちすぎた場合
        if (diffY > maxHeightFromCheckpoint || this.transform.position.y < -fallDistanceFromCheckpoint)
        {
            Debug.Log($"制限エリア外に出ました (DiffY: {diffY:F2}) -> Respawn");
            Respawn();
        }
    }

    public void Respawn()
    {
        if (currentRespawnPoint == null) return;

        // 重力リセット
        if (playerMove != null)
        {
            playerMove.ResetGravity();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.MovePosition(currentRespawnPoint.position);
        }
        else
        {
            transform.position = currentRespawnPoint.position;
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
            currentRespawnPoint = respawnPoints[index];
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
        return currentRespawnPoint;
    }
}
