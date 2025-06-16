using UnityEngine;
using System.Collections;
using Unity.PlasticSCM.Editor.WebApi;

public class Boss_20_Controller : MonoBehaviour, IWallHitTable
{
    public Boss_20_StatusObjectScript status;
    public GameObject Bullet_Prefab;
    public Transform Bullet_Position;

    private int Boss_Move_Direction;
    private float Bullet_Timer;
    private float rest_Timer;
    private float boss_Speed;


    private bool isResting = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>(); // RigidbodyÇéÊìæ
        Initialize();
    }

    void Update()
    {
        HandleShooting();
    }

    void FixedUpdate()
    {
        Move(); // RigidbodyÇìÆÇ©Ç∑Ç»ÇÁ FixedUpdate Ç≈
    }

    void Initialize()
    {
        Bullet_Timer = status.Attack;
        Boss_Move_Direction = status.LEFT;
        rest_Timer = status.Rest;
        boss_Speed = status.Speed;
    }

    void Move()
    {
        if (!isResting)
        {
            // RigidbodyÇÃ MovePosition ÇégÇ¡Çƒà⁄ìÆ
            Vector3 newPosition = rb.position + new Vector3(boss_Speed * Boss_Move_Direction * Time.fixedDeltaTime, 0f);
            rb.MovePosition(newPosition);
            rest_Timer -= Time.fixedDeltaTime;
        }

        if (rest_Timer <= 0f && !isResting)
        { 
            isResting = true;
            StartCoroutine(RestAndResume());
        }
    }

    IEnumerator RestAndResume()
    {
        Debug.Log("å∏ë¨äJén");
        float decelerationRate = 2f;

        while (boss_Speed > 0f)
        {
            boss_Speed = Mathf.MoveTowards(boss_Speed, 0f, decelerationRate * Time.fixedDeltaTime);
            yield return new WaitForFixedUpdate();
        }

        boss_Speed = 0f;
        Debug.Log("í‚é~äÆóπ");
        yield return new WaitForSeconds(3f);
        Debug.Log("ãxåeèIóπ");
        boss_Speed = status.Speed;
        rest_Timer = status.Rest;
        isResting = false;
    }

    public void OnHitWall()
    {
        Debug.Log("ìñÇΩÇËÇ‹ÇµÇΩ");
        if (Boss_Move_Direction != 0 && Boss_Move_Direction == status.LEFT)
        {
            Boss_Move_Direction = status.RIGHT;
        }
        else if (Boss_Move_Direction == status.RIGHT)
        {
            Boss_Move_Direction = status.LEFT;
        }
    }


void HandleShooting()
{
    Bullet_Timer -= Time.deltaTime;
    if (Bullet_Timer <= 0f)
    {
        Bullet();
        Bullet_Timer = status.Attack;
    }
}

void Bullet()
{
    if (Bullet_Prefab != null && Bullet_Position != null)
    {
        Instantiate(Bullet_Prefab, Bullet_Position.position, Bullet_Position.rotation);
    }
}
}
