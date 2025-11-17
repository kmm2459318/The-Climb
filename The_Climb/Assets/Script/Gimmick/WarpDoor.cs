using UnityEngine;

public class WarpDoor : MonoBehaviour
{
    [SerializeField] private bool canGoBack = false;  //そのドアは引き返すことが可能か
    [Header("canGoBackが\ntrueであればgoToDoorを\nfalseならgoToWhereを指定しろ")]
    [SerializeField] private GameObject goToDoor;     //行先のドア
    [SerializeField] private GameObject goToWhere;    //行先の場所
    private bool nearPlayer = false;  //近くにプレイヤーがいるか判定
    private bool savePlayer = false;  //プレイヤーのオブジェクトを取得できているか判定
    private Transform player;         //プレイヤーのtransform
    private PlayerState playerState;

    void Start()
    {
        if ((canGoBack && goToDoor == null) || (!canGoBack && goToWhere == null))
        {
            Debug.LogError(canGoBack ? "goToDoor" : "goToWhere" + "を指定してください。");
        }
    }

    void Update()
    {
        if (nearPlayer && Input.GetKeyDown(KeyCode.W) && playerState.carryingBuddy)
        {
            if (canGoBack)
            {
                player.GetComponent<Rigidbody>().MovePosition(goToDoor.transform.position + new Vector3(0f, -1f, 0f));
            }
            else
            {
                player.GetComponent<Rigidbody>().MovePosition(goToWhere.transform.position);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nearPlayer = true;

            if (!savePlayer)
            {
                player = other.transform;
                playerState = player.GetComponent<PlayerState>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nearPlayer = false;
        }
    }
}
