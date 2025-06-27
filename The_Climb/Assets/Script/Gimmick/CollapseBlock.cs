using Unity.VisualScripting;
using UnityEngine;

public class CollapseBlock : MonoBehaviour
{
    public float CollapseDelay = 1f;
    public float RespawnDelay  = 3f;

    private bool isCollapsing = false;
    private float CollapseTimer;
    private float RespawnTimer;

    private GameObject visualPart;

    void Start()
    {
        visualPart = transform.GetChild(0).gameObject;
        CollapseTimer = CollapseDelay;
    }

    void Update()
    {
        if(isCollapsing)
        {
            Debug.Log("isCollapse内");
            CollapseTimer -= Time.deltaTime;
            if(CollapseTimer <= 0)
            {
                Collapse();
            }
        }

        //非アクティブ中のリスポーン処理
        if (visualPart != null && !visualPart.activeSelf)
        {
            Debug.Log("非アクティブ中のリスポーン処理");
            RespawnTimer -= Time.deltaTime;
            if(RespawnTimer <=0)
            {
                Respawn();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isCollapsing)
        {
            Debug.Log("プレイヤーに当たった");
            isCollapsing = true;
        }
    }

    void Collapse()
    {
        if(visualPart != null)
        {
            visualPart.SetActive(false);
        }

        CollapseTimer = CollapseDelay;
        isCollapsing = false;
        RespawnTimer = RespawnDelay;
    }

    void Respawn()
    {
        Debug.Log("リスポーン関数に入った");
        if(visualPart != null)
        {
            visualPart.SetActive(true);
        }
    }
}
