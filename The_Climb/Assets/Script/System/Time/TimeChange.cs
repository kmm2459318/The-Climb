using UnityEngine;


public class TimeChange : MonoBehaviour
{
    [Header("ステージセレクト")]
    [SerializeField] public GameObject[] Maps;

    public int[] CurrentMapIndex = { 0, 1 }; 　//マップの候補
    public int CurrentActiveIndex = 0;  　　　 //最初に0が選ばれるようにする    

    void start()
    {
        for(int i = 0; i < Maps.Length; i++)
        {
            if(i == CurrentMapIndex[CurrentActiveIndex])
                Maps[i]?.SetActive(true);
            else
                Maps[i]?.SetActive(false);
        }
    }

     void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchToNextMap();
        }
    }

    public void SwitchToNextMap()
    {
        //現在のマップを非表示にする
        Maps[CurrentActiveIndex].SetActive(false);

        //次のマップに切り替え
        CurrentActiveIndex++;
        if(CurrentActiveIndex >= CurrentMapIndex.Length)
        {
            CurrentActiveIndex = 0;
        }
        Maps[CurrentMapIndex[CurrentActiveIndex]].SetActive(true);

    }

}
