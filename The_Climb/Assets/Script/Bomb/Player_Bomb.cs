using UnityEngine;
using UnityEngine.Rendering;

public class Player_Bomb : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] float b_force = 10;
    [SerializeField] float b_radius = 5;
    [SerializeField] float b_upward = 0;
    [SerializeField] float b_time = 3;
    public int b_damage = 5;

    private float b_explosion = 0;
    private bool exploded = false;
    private Vector3 b_pos;

    // 爆発後に呼ばれるコールバック(Bomb_Generate側から登録)
    private System.Action onExploded;

    public void SetOnExplodedCallback(System.Action callback)
    {
        onExploded = callback;
    }

    void Update()
    {
        b_explosion += Time.deltaTime; 

        if (b_explosion >= b_time && !exploded)
        {
            Explosion();
        }
    }

    // Bomb_Generateから呼ばれる強制爆発
    public void ForceExplosion()
    {
        if (!exploded)
        {
            Explosion();
        }
    }

    void Explosion()
    {
        exploded = true;
        b_pos = transform.position;

        PlayParticle();
        ApplyExplosionForce();
        Debug.Log("爆発しました");

        // コールバック(プレイヤー側に爆発したことを知らせる)
        onExploded.Invoke();

        Destroy(gameObject); //爆弾を削除
    }

    //パーティクル
    void PlayParticle()
    {
        Instantiate(explosion, b_pos, Quaternion.identity);
    }

    //爆風
    void ApplyExplosionForce()
    {
        Collider[] hitColliders = Physics.OverlapSphere(b_pos, b_radius);
        
        foreach(var hit in hitColliders)
        {
            var obj = hit.gameObject;
            var rb = obj.GetComponent<Rigidbody>();

            if (rb == null) continue;
            rb.AddExplosionForce(b_force, b_pos, b_radius, b_upward, ForceMode.Impulse);

            ObjExplosionTarget(obj);
        }
    }

    //破棄するobject
    void ObjExplosionTarget(GameObject obj)
    {
        switch (obj.tag)
        {
            case "BreakingWall":
                DestructibleBlock Block = obj.GetComponent<DestructibleBlock>();
                Block.BreakBlock();
                Debug.Log("壁を破壊しました");    
                break;
        
            case "Enemy":
                var enemy = obj.GetComponent<Enemy>();
                if (enemy != null)
                {
                        enemy.TakeDamage(b_damage);
                      Debug.Log("爆風ヒット");
                }
                break;

            default: 
            //何もしない
            break;
        }
    }
}