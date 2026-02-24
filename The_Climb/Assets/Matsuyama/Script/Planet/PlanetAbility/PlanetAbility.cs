using UnityEngine;
using TheClimb.Core;
using UnityEngine.InputSystem;

namespace TheClimb.Astral
{
    public class PlanetAbility : PlanetAbilityBase    //  天体の能力コマンド
    {
        public PlanetAbility(Transform planetTF) : base(planetTF)
        {  }

        public override void ChargePower(InputAction.CallbackContext context)    //  マウス左クリックして力を溜め始めた瞬間の処理
        {
            EffectAPIWindow.Play(new EffectKey(GameMode.Astral, EffectKind.ChargePower), planetTF);
        }
    }
}