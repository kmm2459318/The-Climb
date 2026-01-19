using UnityEngine;

namespace TheClimb.Core
{
    public class ObjectLabelContext    //  オブジェクトラベルコンテキスト
    {

        //  --  外部API
        
        public ObjectLabelContext(Transform mainCamTF, Transform impactBallTF)
        {
            MainCameraTF = mainCamTF;
            ImpactBallTF = impactBallTF;
        }

        public Transform MainCameraTF { get; private set; }    //  メインカメラのトランスフォームプロパティ。
        public Transform ImpactBallTF { get; private set; }    //  衝撃球のトランスフォームプロパティ。
    }
}