using UnityEngine;
using UnityEngine.Audio;

public class Player_Bomb : MonoBehaviour
{
    [SerializeField] GameObject explosion;
    [SerializeField] float b_force = 10;
    [SerializeField] float b_radius = 5;
    [SerializeField] float b_upward = 0;
    [SerializeField] float b_time = 3;
    [SerializeField] AudioClip explosionSound; // 爆発SE
    [SerializeField] private AudioMixerGroup seMixerGroup;
    [SerializeField, Range(0f, 1f)] float explosionVolume = 1f; // 爆発音量
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
        PlayExplosionSound(); // SEを再生
        ApplyExplosionForce();
        // コールバック(プレイヤー側に爆発したことを知らせる)
        onExploded?.Invoke();
        CameraShake.Instance.Shake(0.15f,0.11f);
        Destroy(gameObject); //爆弾を削除
    }

    //パーティクル
    void PlayParticle()
    {
        Instantiate(explosion, b_pos, Quaternion.identity);
    }

    //SE再生
    void PlayExplosionSound()
    {
        if (explosionSound != null)
        {
            // 2Dサウンドとして再生するためのオブジェクト生成
            GameObject audioObj = new GameObject("ExplosionAudio");
            audioObj.transform.position = b_pos;
            AudioSource source = audioObj.AddComponent<AudioSource>();
            
            source.clip = explosionSound;
            source.volume = explosionVolume;
            source.spatialBlend = 0f; // 2Dサウンドに設定
            source.outputAudioMixerGroup = seMixerGroup;
            source.Play();

            // 再生終了後にオブジェクトを破棄
            Destroy(audioObj, explosionSound.length);
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
                Debug.Log("壁を発見");
                DestructibleBlock Block = obj.GetComponent<DestructibleBlock>();
                Block.BreakBlock();
                break;
        
            case "Enemy":
                var enemy = obj.GetComponent<Enemy>();
                if (enemy != null)
                {
                        enemy.TakeDamage(b_damage);
                      Debug.Log("爆風ヒット");
                }
                break;

            case "StalkerHand":
                obj.GetComponent<KidnapBuddy>().handController.ReleaseBuddy();
                break;

            case "BossStalker":
                var boss = obj.transform.parent.gameObject.GetComponent<BossStalkerHandController>();
                boss.BossStalkerSlow();
                break;

            default: 
            //何もしない
            break;
        }
    }
}