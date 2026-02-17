using UnityEngine;
using UnityEngine.InputSystem;

namespace TheClimb.Core
{
    public class SwordCatchInputReceiver : MonoBehaviour    //  入力受付
    {
        private InputSystem_Actions inputActions;
        [SerializeField] GameObject ImpactBall;
        [SerializeField] Transform GenerateTF;
        
        bool IsClearAstral = false;
        
        private void Awake()
        {
            inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            inputActions.Player.Enable();
            inputActions.Player.GenerateImpactBall.started += GenerateImpactBall;
        }
        void Start()
        {
            IsClearAstral = true;
            //if(PlayerPrefs.GetInt("Matsuyama") == 1)
            //{
            //    IsClearAstral = true;
            //}
            //else
            //{
            //    IsClearAstral = false;
            //}
        }

        private void OnDisable()
        {
            inputActions.Player.GenerateImpactBall.started -= GenerateImpactBall;
            inputActions.Player.Disable();
        }

        void GenerateImpactBall(InputAction.CallbackContext ctx)
        {
            if(IsClearAstral)
            Instantiate(ImpactBall, GenerateTF);
        }
    }
}