using TheClimb.UniversalGravity;
using UnityEngine;

public class GravitationTargetEntry
{
    public GravitationTargetMarker target;    //  ターゲットコンポーネント
    public Rigidbody rigidbody;                 //  リジッドボディコンポーネント

    public GravitationTargetEntry(GravitationTargetMarker target, Rigidbody rigidbody)    //  コンストラクタ
    {
        this.target = target;
        this.rigidbody = rigidbody;
    }
}
