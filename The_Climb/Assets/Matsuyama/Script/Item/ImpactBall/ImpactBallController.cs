using UnityEngine;
using TheClimb.Astral;
using TheClimb.UniversalGravity;

namespace TheClimb.Item
{
    public class InpactBallController : MonoBehaviour, IAtractable    //  インパクトボールコントロールクラス
    {
        ItemStateFactory itemStateFactory;    //  ItemのStateを生成する
        ItemStateMachine itemStateMachine;    //  ItemのStateMachine
        GravitationTargetStatusBlock _gravitationTargetStatusBlock;    //  ターゲットのステータスブロック

        public GravitationTargetStatusBlock statProperty => _gravitationTargetStatusBlock;    //  ステータスプロパティ

        public void Initialize(GravitationTargetStatusBlock stat)    //  初期化
        {
            itemStateFactory = new ItemStateFactory();
            itemStateMachine = new ItemStateMachine(itemStateFactory);
            _gravitationTargetStatusBlock = stat;
            
            itemStateMachine.Initialize();    //  ステートマシーン初期化
        }

        public void OnAttracting()    //  引き寄せられた時の処理
        {
            //itemStateMachine.ChangeState(itemStateFactory.CreateState(ItemStateID.Attracting));    //  状態変更
        }
    }
}