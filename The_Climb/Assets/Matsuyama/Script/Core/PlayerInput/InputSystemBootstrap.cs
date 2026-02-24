using TheClimb.Astral;
using UnityEngine;

namespace TheClimb.Core
{
    public class InputSystemBootstrap : MonoBehaviour    //  インプットシステムの初期化用クラス
    {
        InputSystem_Actions inputAction_System;

        [SerializeField] InputHandleBase playerInputHandle;
        [SerializeField] Transform planetTF;
        PlanetAbilityBase planetAbilityBase;

        void Awake()
        {
            inputAction_System = new InputSystem_Actions();
            planetAbilityBase = new PlanetAbility(planetTF) as PlanetAbilityBase;
        }
        void Start()
        {
            playerInputHandle.Initialize(inputAction_System, planetAbilityBase);
        }
    }
}