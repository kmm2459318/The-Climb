using UnityEngine;


public class TimeChange : MonoBehaviour
{
    [Header("ステージセレクト")]
    [SerializeField] private GameObject[] MapPrefabs; // プレハブ登録用
    private GameObject[] MapInstance;   // 生成したマップの参照
    [Header("プレイヤーの参照")]
    [SerializeField] private Transform Player;
    [Tooltip("KeyBindのスクリプト")]
    public KeyBind KeyBind;

    public int[] CurrentMapIndex = { 0, 1 };
    private int CurrentActiveIndex = 0;



    void Start()
    {
        KeyBind = GameObject.Find("KeyManager").GetComponent<KeyBind>();
        // マップを全部生成して非表示にする
        MapInstance = new GameObject[MapPrefabs.Length];
        for (int i = 0; i < MapPrefabs.Length; i++)
        {
            MapInstance[i] = Instantiate(MapPrefabs[i], Vector3.zero, Quaternion.identity);
            MapInstance[i].SetActive(false);
        }

        // 最初のマップだけ有効化
        MapInstance[CurrentActiveIndex].SetActive(true);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyBind.timeSwitch))
        {
            Debug.Log("変わりました");
            SwitchToNextMap();
        }
    }

    public void SwitchToNextMap()
    {
        // 現在のマップを非表示
        MapInstance[CurrentActiveIndex].SetActive(false);

        // 次のマップに進める
        CurrentActiveIndex++;
        if (CurrentActiveIndex >= MapInstance.Length)
        {
            CurrentActiveIndex = 0;
        }

        // 新しいマップを表示
        MapInstance[CurrentActiveIndex].SetActive(true);
    }
}


  





