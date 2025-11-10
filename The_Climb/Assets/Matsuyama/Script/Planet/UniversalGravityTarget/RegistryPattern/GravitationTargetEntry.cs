using TheClimb.UniversalGravity;
using UnityEngine;

public class GravitationTargetEntry
{
    public GravitationTargetRegister target;    //  ターゲットコンポーネント
    public Rigidbody rigidbody;                 //  リジッドボディコンポーネント

    public GravitationTargetEntry(GravitationTargetRegister target, Rigidbody rigidbody)    //  コンストラクタ
    {
        this.target = target;
        this.rigidbody = rigidbody;
    }
}
