using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TheClimb.Core;

namespace TheClimb.Astral
{
    public class PlanetAbility : PlanetAbilityBase   //  天体の能力コマンド
    {
        public PlanetAbility(PlanetAbilityStatsBase stats, Transform planetTF, Transform playerTF) : base(stats, planetTF, playerTF)
        {
            isChargeComplete = false;
            chargeCoroutine = null;
        }

        public override void ChargeAbility(InputAction.CallbackContext context)    //  能力チャージコルーチンを作動させる受け子関数
        {
            chargeCoroutine = ServiceLocator.Resolve<ICoroutineRunnerFacade>().StartCoroutine(ChargePower());
        }

        IEnumerator ChargePower()    //  能力チャージの挙動処理
        {
            float holdTime = 0f;

            while ((holdTime += Time.deltaTime) < abilityStats.PrimaryEffectSpawnTime)    //  チャージ開始エフェクト生成まで待機
            { yield return null; }
            EffectAPIWindow.Play(new EffectKey(GameMode.Astral, EffectKind.AwakePower), planetTF);

            while ((holdTime += Time.deltaTime) < abilityStats.SecondaryEffectSpawnTime)    //  二段階目のエフェクト生成まで待機
            { yield return null; }
            EffectAPIWindow.Play(new EffectKey(GameMode.Astral, EffectKind.ChargePower), planetTF);
            EffectAPIWindow.StopSudden(new EffectKey(GameMode.Astral, EffectKind.AwakePower));

            isChargeComplete = true;   
        }

        public override void BurstChargeForce(InputAction.CallbackContext context)
        {
            if (!isChargeComplete)
            {
                ServiceLocator.Resolve<ICoroutineRunnerFacade>().StopCoroutine(chargeCoroutine);
                EffectAPIWindow.Stop(new EffectKey(GameMode.Astral, EffectKind.AwakePower));
            }
            else
            {
                Vector3 vectorToPlanet = planetTF.transform.position - playerTF.transform.position;
                Vector3 blowForce = vectorToPlanet * abilityStats.RepulsiveFouce;
                Debug.Log(blowForce);
                ServiceLocator.Resolve<PlayerAPIFacadeBase>().AddForce(vectorToPlanet, AddForceMode.Force);
            }
            isChargeComplete = false;
        }

    }
}