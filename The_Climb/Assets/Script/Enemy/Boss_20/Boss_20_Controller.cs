using TMPro;
using UnityEngine;

public class Boss_20_Controller : MonoBehaviour
{

    public Boss_20_StatusObjectScript status;  �@//�{�X�̃X�e�[�^�X
    public GameObject bullet_Prefab;�@�@�@�@�@�@ //�������U����Prehab; 
    public Transform bullet_Position;            //�������U���̈ʒu

    private float bullet_Timer;             �@�@ //�������U���̃^�C�~���O
    private Vector3 targetPosition;�@�@�@�@�@�@�@//�{�X�̈ʒu
    private float rest_Timer;�@�@�@�@�@�@�@�@�@�@//�{�X�̋x�e����
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
    
    //�{�X�̓���
    void Move()�@�@�@�@�@�@�@�@�@�@�@�@�@
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        float moveSpeed = status.Speed * Time.deltaTime;

        transform.position += moveDirection * moveSpeed;

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            SetNewTargetPosition();
        }
    }

    //�G�̈ړ��͈�
    void SetNewTargetPosition()
    {
        float offsetX = Random.Range(-status.Lateral, status.Lateral);
        targetPosition = transform.position + new Vector3(offsetX, 0, 0);
    }

    //�������U���̍U���^�C�~���O
    void HandleShooting()
    {
        bullet_Timer -= Time.deltaTime;
        if (bullet_Timer <= 0f)
        {
            Bullet();
            bullet_Timer = status.Attack;
        }
    }

    //�������U���̔���
    void Bullet()

    {
        if ( bullet_Prefab != null && bullet_Position != null)
        {
            Instantiate( bullet_Prefab, bullet_Position.position, bullet_Position.rotation);
        }
    }
}
