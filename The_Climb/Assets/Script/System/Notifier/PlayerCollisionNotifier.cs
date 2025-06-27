using UnityEngine;

//  プレイヤーへの衝突を通知
public class PlayerCollisionNotifier : CollisionNotifier<IBlowable>
{
    void OnCollisionEnter(Collision collision)
    {
        Vector3 Direction = collision.transform.position - transform.position;    //  吹き飛ばし方向

        //  壁に当たった時の処理を実行
        NotifyIfTagMatches(collision, "Player", h => h.Blow(collision.rigidbody, Direction));
    }
}
