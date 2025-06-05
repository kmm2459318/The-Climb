using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
//  敵キャラクターを実際に動かすスクリプト
public class EnemyMover : MonoBehaviour
{
    Rigidbody RB;    //  リジッドボディインスタンス
    void Awake()
    {
        RB = GetComponent<Rigidbody>();
    }
    //  移動
    public void Move(Vector3 Velocity)
    {
        RB.MovePosition(RB.position + Velocity);
    }
}
