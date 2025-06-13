using UnityEngine;

public class Boss_20_Controller : MonoBehaviour, IWallHitTable
{
    public Boss_20_StatusObjectScript status;
    public GameObject Bullet_Prefab;
    public Transform Bullet_Position;
    EnemyMover enemyMover;

    private int Boss_Move_Direction;
    private float Bullet_Timer;
    private float rest_Timer;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>(); // Rigidbody‚ðŽæ“¾
        enemyMover = GetComponent<EnemyMover>();
        Initialize();
    }

    void Update()
    {
        HandleShooting();
    }

    void FixedUpdate()
    {
        Move(); // Rigidbody‚ð“®‚©‚·‚È‚ç FixedUpdate ‚Å
    }

    void Initialize()
    {
        Bullet_Timer = status.Attack;
        Boss_Move_Direction = status.LEFT;
        rest_Timer = 0;
    }

    void Move()
    {
        // Rigidbody‚Ì MovePosition ‚ðŽg‚Á‚ÄˆÚ“®
        Vector3 newPosition = rb.position + new Vector3(status.Speed * Boss_Move_Direction * Time.fixedDeltaTime, 0f);
        rb.MovePosition(newPosition);
        
    }

    public void OnHitWall()
    {
        Debug.Log("“–‚½‚è‚Ü‚µ‚½");
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
