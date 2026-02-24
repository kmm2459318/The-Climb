using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Bomb_Generate : MonoBehaviour
{
    [SerializeField] GameObject bombPrefab; 
    [SerializeField] private PlayerMove p_move; 
    [SerializeField] private Bomb_Aim p_aim; 
    public int shoot_power = 10000;

    // 投げた爆弾を保持
    private Player_Bomb currentBomb;

    void Update()
    {
        if (Time.timeScale == 0f) return;
        p_aim.UpdateDirection(p_move.MoveInput);
        //爆弾発射
        if (Input.GetMouseButtonDown(0))
        {
            if (currentBomb == null)
            {
                ShootBomb();
            }
            else
            {
                // 爆弾が存在しているなら強制爆発
                currentBomb.ForceExplosion();
            }
        }
    }

    private void LateUpdate()
    {
        // ローカルX座標とローカルY座標を0に固定
        Vector3 pos = transform.localPosition;
        pos.x = 0;
        pos.y = 0;
        transform.localPosition = pos;
    }

    //爆弾の処理
    private void ShootBomb()
    {
        Transform spawnPos = p_aim.GetSpawnPos();
        Vector3 shootDir = p_aim.GetShootDirection();

        GameObject bombObj = Instantiate(bombPrefab, spawnPos.position, Quaternion.identity);

        // 生成した爆弾のPlayer_Bombを保持
        currentBomb = bombObj.GetComponent<Player_Bomb>();
        currentBomb.SetOnExplodedCallback(() =>
        {
            currentBomb = null;
        });

        Rigidbody rb = bombObj.GetComponent<Rigidbody>();
        rb.AddForce(shootDir * shoot_power);
    }
}
