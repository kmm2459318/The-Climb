using UnityEngine;
using UnityEngine.Rendering;

public class Player_Bomb : MonoBehaviour
{
    [SerializeField] GameObject b_explosionEffect;
    [SerializeField] float b_force = 10;
    [SerializeField] float b_radius = 5;
    [SerializeField] float b_upward = 0;
    [SerializeField] float b_time = 3;
    public int b_damage = 5;

    private float b_explosion = 0;
    private bool exploded = false;
    private Vector3 b_pos;

    void Update()
    {
        b_explosion += Time.deltaTime; 

        if (b_explosion >= b_time && !exploded)
        {
            exploded = true;
            Explosion();
        }
    }

    void Explosion()
    {
        b_pos = transform.position;
        ApplyExplosionForce();
        Debug.Log("爆発しました");
        Destroy(gameObject); //削除
        PlayParticle();
    }

    //エフェクト
    void PlayParticle()
    {
        if (b_explosionEffect != null)
        {
           GameObject effect = Instantiate(b_explosionEffect, b_pos, Quaternion.identity);
        }
        
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
                Vector3 dir = (rb.position - b_pos).normalized;
                dir.y *= 0.5f; // ← 上方向を弱める（0.0〜1.0）

                float dist = Vector3.Distance(rb.position, b_pos);
                float atten = 1f - Mathf.Clamp01(dist / b_radius);

                rb.AddForce(dir * b_force * atten, ForceMode.Impulse);                  

            ObjExplosionTarget(obj);
        }
    }

    //破棄するobject
    void ObjExplosionTarget(GameObject obj)
    {
        switch (obj.tag)
        {
          case "BreakingWall":
            Destroy(obj);
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