using TMPro;
using UnityEngine;

public class Boss_20_Controller : MonoBehaviour
{

    public Boss_20_StatusObjectScript status;  　//ボスのステータス
    public GameObject bullet_Prefab;　　　　　　 //遠距離攻撃のPrehab; 
    public Transform bullet_Position;            //遠距離攻撃の位置

    private float bullet_Timer;             　　 //遠距離攻撃のタイミング
    private Vector3 targetPosition;　　　　　　　//ボスの位置
    private float rest_Timer;　　　　　　　　　　//ボスの休憩時間
    void Start()
    {
        SetNewTargetPosition();
        bullet_Timer = status.Attack;
    }

    void Update()
    {
        Move();
        HandleShooting();
    }
    
    //ボスの動き
    void Move()　　　　　　　　　　　　　
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float moveSpeed = status.Speed * Time.deltaTime;

        transform.position += moveDirection * moveSpeed;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewTargetPosition();
        }
    }

    //敵の移動範囲
    void SetNewTargetPosition()
    {
        float offsetX = Random.Range(-status.Lateral, status.Lateral);
        targetPosition = transform.position + new Vector3(offsetX, 0, 0);
    }

    //遠距離攻撃の攻撃タイミング
    void HandleShooting()
    {
        bullet_Timer -= Time.deltaTime;
        if (bullet_Timer <= 0f)
        {
            Bullet();
            bullet_Timer = status.Attack;
        }
    }

    //遠距離攻撃の発射
    void Bullet()

    {
        if ( bullet_Prefab != null && bullet_Position != null)
        {
            Instantiate( bullet_Prefab, bullet_Position.position, bullet_Position.rotation);
        }
    }
}
