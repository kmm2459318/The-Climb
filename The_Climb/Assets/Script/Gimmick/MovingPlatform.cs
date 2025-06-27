using UnityEngine;

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

    private Vector3 StartPosition;
    private Rigidbody Rigidbody;
    private Rigidbody PlayerRigidbody; //プレイヤーのRigidbodyを保持
    private float ElapsedTime = 0f;

    private Vector3 PreviousPosition;
    private bool IsPlayerOnTop = false;

    private void Start()
    {
        StartPosition = transform.position;
        Rigidbody = GetComponent<Rigidbody>();
        PreviousPosition = transform.position;
    }

    void FixedUpdate()
    {
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
}
