using UnityEngine;
using UnityEngine.InputSystem;

namespace TheClimb.Astral
{
    public abstract class PlanetAbilityBase    //  差し替えするためと、コンテキスト共通処理吸い上げ用
    {
        protected Transform planetTF;

        protected PlanetAbilityBase(Transform planetTF)
        {
            this.planetTF = planetTF; 
        }

        public abstract void ChargePower(InputAction.CallbackContext context);
    }
}