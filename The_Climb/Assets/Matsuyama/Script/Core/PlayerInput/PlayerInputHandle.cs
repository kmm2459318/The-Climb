//namespace TheClimb.Core
//{
//    public class PlayerInputHandle : InputHandleBase    //  プレイヤーの入力を受けつけるクラス
//    {
//        private InputSystem_Actions inputSystem_Action;

//        bool IsClearAstral = false;

//        public PlayerInputHandle(InputSystem_Actions inputSystem)
//        {
//            inputSystem_Action = inputSystem;

//            SetReaction();
//        }

//        void SetReaction()
//        {
//            inputSystem_Action.Player.Enable();
//            inputSystem_Action.Player.GenerateImpactBall.started += GenerateImpactBall;
//        }

//        void SetOffReaction()
//        {
//            inputSystem_Action.Player.GenerateImpactBall.started -= GenerateImpactBall;
//            inputSystem_Action.Player.Disable();
//        }
//    }
//}
////if(PlayerPrefs.GetInt("Matsuyama") == 1)
////{
////    IsClearAstral = true;
////}
////else
////{
////    IsClearAstral = false;
////}