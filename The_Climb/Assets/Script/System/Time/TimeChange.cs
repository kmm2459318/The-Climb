using UnityEngine;


public class TimeChange : MonoBehaviour
{
    [Header("ステージセレクト")]
    [SerializeField] private GameObject[] MapPrefabs; // プレハブ登録用

    public int[] CurrentMapIndex = { 0, 1 };
    private int CurrentActiveIndex = 0;

    private GameObject CurrentMapInstance; // 生成したマップの参照

    void Start()
    {
        LoadMap(CurrentMapIndex[CurrentActiveIndex]);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("変わりました");
            SwitchToNextMap();
        }
    }

    public void SwitchToNextMap()
    {
        // 現在のマップを削除
        if (CurrentMapInstance != null)
        {
            Destroy(CurrentMapInstance);
        }

        // 次のインデックスに進める
        CurrentActiveIndex++;
        if (CurrentActiveIndex >= CurrentMapIndex.Length)
        {
            CurrentActiveIndex = 0;
        }

        // 新しいマップを生成
        LoadMap(CurrentMapIndex[CurrentActiveIndex]);
    }

 
    private void LoadMap(int MapIndex)
    {
        if (MapIndex >= 0 && MapIndex < MapPrefabs.Length)
        {
            CurrentMapInstance = Instantiate(MapPrefabs[MapIndex], Vector3.zero, Quaternion.identity);
        }

        else
        {
            Debug.LogWarning("指定されたマップインデックスが範囲外です: " + MapIndex);
        }
    }
}


