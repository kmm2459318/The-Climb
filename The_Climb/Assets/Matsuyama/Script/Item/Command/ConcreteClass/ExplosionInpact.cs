using System.Collections;
using TheClimb.Core;
using UnityEngine;

namespace TheClimb.Item
{
    public class ExplosionInpact : ItemCommandBase    //  衝撃波を炸裂させる
    {
        ImpactBallContext _ctx;                //  衝撃球コンテキスト
        ImpactBallRuntimeData _runtimeData;    //  衝撃球コンテキスト
        Transform _playerTransform;    //  プレイヤーのトランスフォーム
        Transform _planetTransform;    //  プレイヤーのトランスフォーム
        Rigidbody playerRigidBody;    //  プレイヤーのリジッドボディ

        ICommandContext _commandContext;
        IItemStateFactory _itemStateFactory;
        ICorutineRunner _corutineRunner;   //  コルーチンランナー
        IItemStateContext _ItemStateContext;

        public ExplosionInpact(ImpactBallContext ctx, IItemStateFactory stateFactory, ICorutineRunner corutineRunner, IImpactable targetPlayer)    //  コンストラクタ
        {
            _ctx = ctx;
            _runtimeData = ctx.RuntimeData;

            _itemStateFactory = stateFactory;
            _corutineRunner = corutineRunner;
            _planetTransform = ctx.Transform;
            _playerTransform = targetPlayer.TransformGetter;
            playerRigidBody = targetPlayer.RigidbodyGetter;
        }
        public void InjectContext(ICommandContext commandContext, IItemStateContext itemStateContext)    //  コンテキスト注入
        {
            _commandContext = commandContext;
            _ItemStateContext = itemStateContext;
        }
        
        public override void Execute()    //  衝撃波炸裂実行
        {
            _corutineRunner.StartCoroutine(ExplosionImapct());
        }

        IEnumerator ExplosionImapct()    //  衝撃を炸裂させる    //  距離で制限・当たり判定無効化時間調整・エフェクト調整
        {
            float ExplosionForce = _ctx.ConfigSO.ExplosionForce;     //  爆発の衝撃力
            float ExplosionRange = _ctx.ConfigSO.ExplosionRadius;    //  爆発の半径
            
            _playerTransform.gameObject.layer = LayerMask.NameToLayer("BlowingPlayer");

            Debug.Log("kaboom");
            while ((_runtimeData._RemainingExplosionTime -= Time.deltaTime) > 0)
            {
                Vector3 BlowForce = (_playerTransform.position - _planetTransform.position) * ExplosionForce;

                if (playerRigidBody.linearVelocity.y < 40f && Vector3.Distance(_ctx.Transform.position, _playerTransform.position) < _ctx.ConfigSO.ExplosionRadius)
                {
                    Debug.Log("in");
                    playerRigidBody.AddForce(BlowForce, ForceMode.Force);
                }
                yield return null;
            }

            while(playerRigidBody.linearVelocity.y > 0)
            {
                yield return null;
            }

            _playerTransform.gameObject.layer = LayerMask.NameToLayer("Player");
            
            //_commandContext.ChangeState(_itemStateFactory.CreateState(ItemStateID.Idle), _ItemStateContext);
        }
    }
}