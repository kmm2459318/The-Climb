using UnityEngine;

namespace TheClimb.Item
{
    [System.Serializable]
    public class ImpactBallStatusBlock    //  インパクトボールのステータスブロック
    {
        [Header("爆発までの秒数")]
        public int ExplosionCount;    //  爆発までの秒数
    }
}