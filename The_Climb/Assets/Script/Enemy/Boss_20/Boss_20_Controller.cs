using UnityEngine;

public class Boss_20_Controller : MonoBehaviour
{

    public Boss_20_StatusObjectScript status;  　//ボスのステータス
    public GameObject bullet_Prehab;　　　　　　 //遠距離攻撃のPrehab; 
    public Transform breth_Position;             //遠距離攻撃の位置

    private Vector3 targetposition;         
    private float bullet_Timer;
    private float rest_Timer;
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
