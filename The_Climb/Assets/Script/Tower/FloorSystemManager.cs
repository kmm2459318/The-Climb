using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FloorSystemManager : MonoBehaviour
{
    [Header("シーンに配置済みのプレイヤー")]
    public GameObject player;

    public Image fadeImage;

    [Header("階層Prefabリスト")]
    public List<GameObject> floorPrefabs;

    [Header("階層親")]
    public Transform floorsRoot;

    [Header("フェード時間")]
    public float fadeDuration = 1f;

    [Header("トリガー距離")]
    public float triggerDistance = 1.5f;

    private int currentFloorIndex = 0;
    private GameObject currentFloor;
    private FloorData currentFloorData;

    private bool isFirstLoad = true;
    private bool isSwitching = false;

    private void Awake()
    {
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 1);
    }

    private void Start()
    {
        LoadFloor(currentFloorIndex, true);
        StartCoroutine(FadeIn(fadeDuration / 2f));
    }

    private void Update()
    {
        if (isSwitching) return;
        if (player == null || currentFloorData == null) return;

        Vector3 playerPos = player.transform.position;

        // 戻る判定（StartPointに近いとき）
        if (currentFloorData.startPoint != null)
        {
            float distStart = Vector3.Distance(playerPos, currentFloorData.startPoint.position);
            if (distStart <= triggerDistance)
            {
                Debug.Log("[FloorSystemManager] StartPointに到達");
                if (currentFloorIndex > 0)
                    StartCoroutine(SwitchFloorRoutine(currentFloorIndex - 1, false));
                return;
            }
        }

        // 進む判定（GoalPointに近いとき）
        if (currentFloorData.goalPoint != null)
        {
            float distGoal = Vector3.Distance(playerPos, currentFloorData.goalPoint.position);
            if (distGoal <= triggerDistance)
            {
                Debug.Log("[FloorSystemManager] GoalPointに到達");
                if (currentFloorIndex + 1 < floorPrefabs.Count)
                    StartCoroutine(SwitchFloorRoutine(currentFloorIndex + 1, true));
                return;
            }
        }
    }

    IEnumerator SwitchFloorRoutine(int nextIndex, bool goingUp)
    {
        if (isSwitching) yield break;
        isSwitching = true;

        // プレイヤー非アクティブ化
        player.SetActive(false);

        yield return StartCoroutine(FadeOut(fadeDuration / 2f));

        if (currentFloor != null)
            Destroy(currentFloor);

        currentFloorIndex = nextIndex;

        currentFloor = Instantiate(floorPrefabs[currentFloorIndex], floorsRoot);
        currentFloorData = currentFloor.GetComponent<FloorData>();
        if (currentFloorData == null)
        {
            Debug.LogError($"[FloorSystemManager] FloorData コンポーネントがありません: {floorPrefabs[currentFloorIndex].name}");
            isSwitching = false;
            yield break;
        }
        currentFloorData.CheckReferences();

        Transform spawnPos = (isFirstLoad || goingUp) ? currentFloorData.spawnPoint : currentFloorData.goalPoint;
        if (spawnPos != null)
            player.transform.position = spawnPos.position;
        else
        {
            Debug.LogWarning("SpawnPoint または GoalPoint が未設定です");
            player.transform.position = Vector3.zero;
        }

        player.SetActive(true);

        isFirstLoad = false;

        yield return StartCoroutine(FadeIn(fadeDuration / 2f));

        isSwitching = false;
    }

    void LoadFloor(int index, bool goingUp)
    {
        if (index < 0 || index >= floorPrefabs.Count) return;

        currentFloorIndex = index;

        currentFloor = Instantiate(floorPrefabs[index], floorsRoot);
        currentFloorData = currentFloor.GetComponent<FloorData>();

        if (currentFloorData == null)
        {
            Debug.LogError($"[FloorSystemManager] FloorData コンポーネントがありません: {floorPrefabs[index].name}");
            return;
        }

        currentFloorData.CheckReferences();

        Transform spawnPos = (isFirstLoad || goingUp) ? currentFloorData.spawnPoint : currentFloorData.goalPoint;
        if (spawnPos != null)
        {
            player.transform.position = spawnPos.position;
        }
        else
        {
            Debug.LogWarning("SpawnPoint または GoalPoint が未設定です");
            player.transform.position = Vector3.zero;
        }

        // プレイヤーは最初からアクティブなので操作なし

        isFirstLoad = false;
    }

    IEnumerator FadeOut(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 1);
    }

    IEnumerator FadeIn(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - t / duration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.color = new Color(0, 0, 0, 0);
    }
}
