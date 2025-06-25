using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public List<GameObject> floorPrefabs;
    public Transform floorsRoot;
    public GameObject player;
    public FadeController fadeController;

    private int currentFloorIndex = 0;
    private GameObject currentFloor;

    void Start()
    {
        LoadFloor(currentFloorIndex);
    }

    private void InjectDependencies(GameObject floor)
    {
        FloorTransitionPortal[] portals = floor.GetComponentsInChildren<FloorTransitionPortal>();
        foreach (var portal in portals)
        {
            portal.floorManager = this;
            portal.player = player.transform;
        }
    }


    public void LoadFloor(int index)
    {
        if (index < 0 || index >= floorPrefabs.Count) return;
        StartCoroutine(SwitchFloorRoutine(index));
    }

    private IEnumerator SwitchFloorRoutine(int nextIndex)
    {
        // フェードアウト
        yield return StartCoroutine(fadeController.FadeOut());

        // 現在のフロアを削除
        if (currentFloor != null)
        {
            Destroy(currentFloor);
        }

        // 新しいフロアを生成
        currentFloorIndex = nextIndex;
        currentFloor = Instantiate(floorPrefabs[nextIndex], floorsRoot);

        // 🔽 ここでポータルの参照を注入！
        InjectDependencies(currentFloor);

        // プレイヤーをStartPointへ移動
        Transform startPoint = currentFloor.transform.Find("StartPoint");
        if (startPoint != null)
        {
            player.transform.position = startPoint.position;
        }

        // フェードイン
        yield return StartCoroutine(fadeController.FadeIn());
    }

    public void MoveToNextFloor()
    {
        if (currentFloorIndex + 1 >= floorPrefabs.Count) return;
        LoadFloor(currentFloorIndex + 1);
    }

    public void MoveToPreviousFloor()
    {
        if (currentFloorIndex - 1 < 0) return;
        LoadFloor(currentFloorIndex - 1);
    }
}
