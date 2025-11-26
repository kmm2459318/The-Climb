using UnityEngine;

public class LaserKill : MonoBehaviour
{
    [Header("死亡判定するプレイヤー")]
    public GameObject player;

    [Header("レーザー移動設定（往復移動）")]
    public bool moveHorizontal = false;
    public bool moveVertical = false;
    public float speed = 2f;
    public float moveDistance = 3f;

    [Header("ランダム移動設定（中心に戻るタイプ）")]
    public bool moveRandom = false;
    public float randomMoveRadius = 3f;
    public float randomMoveSpeed = 2f;
    public float returnToCenterSpeed = 3f;
    public float randomMoveTime = 1.5f;
    public static System.Action OnPlayerDied;

    [Header("リセットしたいボタン")]
    public ButtonGimmick buttonGimmick;


    private Vector3 startPos;

    // ランダム移動関連
    private Vector3 randomTargetPos;
    private float moveTimer;
    private bool isReturning = false;

    private PlayerRespawnUmeda playerRespawn; //  Respawnスクリプト参照保持

    private void Start()
    {
        startPos = transform.position;
        PickRandomTarget();

        // プレイヤーから PlayerRespawnUmeda を取得
        if (player != null)
        {
            playerRespawn = player.GetComponent<PlayerRespawnUmeda>();
            if (playerRespawn == null)
            {
                Debug.LogWarning(" PlayerRespawnUmeda がプレイヤーに付いていません！");
            }
        }
    }

    private void Update()
    {
        if (moveRandom)
            RandomMoveWithReturn();
        else
            MoveLaser();
    }

    private void MoveLaser()
    {
        Vector3 pos = startPos;

        if (moveHorizontal)
            pos.x += Mathf.Sin(Time.time * speed) * moveDistance;

        if (moveVertical)
            pos.y += Mathf.Sin(Time.time * speed) * moveDistance;

        transform.position = pos;
    }

    private void RandomMoveWithReturn()
    {
        if (!isReturning)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                randomTargetPos,
                randomMoveSpeed * Time.deltaTime
            );

            moveTimer -= Time.deltaTime;

            if (moveTimer <= 0f ||
                Vector3.Distance(transform.position, randomTargetPos) < 0.1f)
            {
                isReturning = true;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                startPos,
                returnToCenterSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, startPos) < 0.1f)
            {
                isReturning = false;
                PickRandomTarget();
            }
        }
    }

    private void PickRandomTarget()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        randomTargetPos = startPos + new Vector3(dir.x, dir.y, 0) * randomMoveRadius;
        moveTimer = randomMoveTime;
    }

    //  プレイヤーと当たったら Respawn() を呼ぶ
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player || other.transform.root.gameObject == player)
        {
            // バリアコンポーネントの取得（子オブジェクトも検索）
            var barrier = player.GetComponentInChildren<PlayerBarrier>();

            // バリアで防げるか試行
            if (barrier != null && barrier.TryBlockAttack())
            {
                return; // 防いだのでリスポーンしない
            }

            Debug.Log("レーザーに触れた → Respawn() を実行");

            if (playerRespawn != null)
            {
                playerRespawn.SendMessage("Respawn", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                Debug.LogWarning(" PlayerRespawnUmeda がプレイヤーに付いていません！");
            }

            // ボタンを戻す処理
            if (buttonGimmick != null)
            {
                buttonGimmick.ForceResetButton();
            }


        }
    }
}
