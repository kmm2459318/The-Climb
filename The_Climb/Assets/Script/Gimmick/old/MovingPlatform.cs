using UnityEngine;
using UnityEngine.UIElements;

public class MovingPlatform : MonoBehaviour
{
    public enum MoveDirection
    {
        Horizontal,
        Vertical,
    }

    public MoveDirection Direction = MoveDirection.Horizontal;
    public float MoveDistance = 3f;
    public float MoveSpeed = 2f;
    public float MovementInfluence = 0.5f;

    //[SerializeField] private TimeGimmickBridge Bridge; // スイッチと状態共有用ブリッジ

    private Vector3 StartPosition;
    private Rigidbody Rigidbody;
    private Rigidbody PlayerRigidbody; //プレイヤーのRigidbodyを保持
    private float ElapsedTime = 0f;

    private Vector3 PreviousPosition;
    private bool IsPlayerOnTop = false;
    //private bool IsActive = false;

    private void Awake()
    {
        StartPosition = transform.position;
        Rigidbody = GetComponent<Rigidbody>();
        PreviousPosition = transform.position;
        //Bridge = GetComponent<TimeGimmickBridge>();

        //if (Bridge != null)
        //{
        //    Bridge.OnStateApplied.AddListener(ApplyState);
        //}
    }

    private void Start()
    {
        //シーン開始時に保存された状態を反映
        //Bridge?.ApplySavedState();
    }

    void FixedUpdate()
    {
        // スイッチが押されていない間は停止
        //if (!IsActive) return; 

        ElapsedTime += Time.fixedDeltaTime;

        Vector3 moveAxis = (Direction == MoveDirection.Horizontal) ? Vector3.right : Vector3.up;
        float offset = Mathf.PingPong(ElapsedTime * MoveSpeed, MoveDistance);
        Vector3 targetPos = StartPosition + moveAxis * offset;

        Vector3 delta = targetPos - PreviousPosition;

        //移動処理
        Rigidbody.MovePosition(targetPos);

        if(PlayerRigidbody != null && IsPlayerOnTop)
        {
            Vector3 adjustedDelta = delta * MovementInfluence;
            PlayerRigidbody.MovePosition(PlayerRigidbody.position + adjustedDelta);
        }

        //次フレームのために保存
        PreviousPosition = targetPos;
        IsPlayerOnTop = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            foreach(ContactPoint contact in collision.contacts)
            {
                //足場の上方向に近い法線か(上にのっているか)
                if(Vector3.Dot(-contact.normal, Vector3.up) > 0.5f)
                {
                    Debug.Log("プレイヤーが動く足場の上に乗った");
                    PlayerRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                    IsPlayerOnTop = true;
                    break;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(PlayerRigidbody != null && PlayerRigidbody.gameObject == collision.gameObject)
            {
                PlayerRigidbody = null;
                IsPlayerOnTop = false;
            }
        }
    }

    //private void ApplyState(bool isActive)
    //{
    //    IsActive = isActive;
    //    Debug.Log("動く足場が起動");
    //}
}
