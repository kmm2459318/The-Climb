using System;

namespace TheClimb.Item
{
    public class ItemEventBus   //  アイテムのイベントバス
    {
        public static event Action onAttractiong;      //  引き寄せられた時
        public static event Action onExplosion;    //  爆発タイマーを超えた時

        //public static event Action OnCatchSuccess;

        public static void OnAttractingStart()    //  引き寄せが始まった時の関数
        {
            onAttractiong?.Invoke();    //  サブスク発火
        }

        public static void OnOverExplosionTimer()    //  爆発タイマーをオーバーしたとき
        {
            onExplosion?.Invoke();
        }
        //public static void CatchSuccess()    //  キャッチ成功時の処理
        //{
        //    OnCatchSuccess?.Invoke();
        //}
    }
}