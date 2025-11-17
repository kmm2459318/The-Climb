using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Bomb_Generate : MonoBehaviour
{
    
    [SerializeField] GameObject bombPrefab;
    [SerializeField] private PlayerMove p_move;
    [SerializeField] private Transform left_pos;
    [SerializeField] private Transform right_pos;
    public int shoot_power = 10000;
    

    //プレイヤーが最後に向いた方向
    private int last_Direction = 0;

    void Update()
    {
        //プレイヤーがどっちを向いたか
        if(p_move.MoveInput < 0)
        {
            last_Direction = -1;
        }
        else if(p_move.MoveInput > 0)
        {
            last_Direction = 1;
        }

        //爆弾発射
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ShootBomb();
        }
    }

    //爆弾の処理
    private void ShootBomb()
    {
        float p_Input = p_move.MoveInput;
        Transform spawn_pos;
        Vector3 direction;

        //プレイヤーが左方向を向いているとき
        if(last_Direction < 0)
        {
            spawn_pos = left_pos;
            direction = -left_pos.right;
        }
        //プレイヤー右方向を向いているとき
        else
        {
            spawn_pos = right_pos;
            direction = right_pos.right;
        }

        GameObject bomb = Instantiate(bombPrefab, spawn_pos.position, Quaternion.identity);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        rb.AddForce(direction * shoot_power);

    }
}
