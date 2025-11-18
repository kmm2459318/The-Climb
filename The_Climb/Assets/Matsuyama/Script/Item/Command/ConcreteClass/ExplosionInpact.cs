using System.Collections;
using TheClimb.Core;
using TheClimb.Player;
using UnityEngine;

namespace TheClimb.Item
{
    public class ExplosionInpact : ItemCommandBase    //  衝撃波を炸裂させる
    {
        Transform PlayerTransform;    //  プレイヤーのトランスフォーム
        Transform _impactBallTF;    //  衝撃球トランスフォーム
        Rigidbody playerRigidBody;    //  プレイヤーのリジッドボディ
        ImpactBallStatusBlock _imapctBallStatusBlock;    //  インパクトボールステータスブロック

        ICommandContext _commandContext;
        IItemStateFactory _itemStateFactory;
        ICorutineRunner _corutineRunner;   //  コルーチンランナー
        public ExplosionInpact(ImpactBallStatusBlock stat, Transform impactBallTF, IItemStateFactory stateFactory, ICorutineRunner corutineRunner, IPlayerDataProvider playerDataProvider)    //  コンストラクタ
        {
            _imapctBallStatusBlock = stat;
            _impactBallTF = impactBallTF;
            PlayerTransform = playerDataProvider.TransformProperty;
            playerRigidBody = playerDataProvider.RigidbodyProperty;

            _itemStateFactory = stateFactory;
            _corutineRunner = corutineRunner;
        }
        public void InjectContext(ICommandContext commandContext)    //  コンテキスト注入
        {
            _commandContext = commandContext;
        }
        
        public override void Execute()    //  衝撃波炸裂実行
        {
            _corutineRunner.StartCoroutine(ExplosionImapct());
        }

        IEnumerator ExplosionImapct()    //  衝撃を炸裂させる
        {
            float Duration = _imapctBallStatusBlock.ExplosionDuration;
            float Elapsed = 0f;

            Debug.Log("kaboom");
            while (Elapsed < Duration)
            {
                Vector3 BlowForce = (PlayerTransform.position - _impactBallTF.position) * _imapctBallStatusBlock.InpactForce;
                
                playerRigidBody.AddForce(BlowForce, ForceMode.Acceleration);

                Elapsed += Time.deltaTime;
                yield return null;
            }

            _commandContext.ChangeState(_itemStateFactory.CreateState(ItemStateID.Idle));
        }
    }
}