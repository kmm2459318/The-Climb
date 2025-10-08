using UnityEngine;
using TheClimb.Astral;

namespace TheClimb.UniversalGravity
{
    public class GravitationTargetAttracter : MonoBehaviour    //  万有引力影響対象のオブジェクトを引き寄せるするスクリプト
    {
        [SerializeField] PlanetStatus planetStatus;         //  天体のステータス群
        PlanetStatusBlock planetStatusBlock;                //  天体のステータスクブロック

        public GravitationLevel CurrentGravitationLevel;    //  重力レベル

        float CurrentAttractRange;    //  現在の万有引力の影響半径
        float CurrentPlanetMass;    //  現在の万有引力の強さ

        void Awake()
        {
            planetStatusBlock = planetStatus.GetStats(PlanetIDs.Earth);    //  地球のステータス取得

            CurrentGravitationLevel = planetStatusBlock.gravitationLevel;

            CurrentPlanetMass = planetStatusBlock.Mass;
            CurrentAttractRange = planetStatusBlock.AttractRange;
        }

        void FixedUpdate()
        {
            AttractTarget();    //  ターゲット引き寄せ
        }
        void AttractTarget()    //  ターゲットを引き寄せる
        {
            foreach (GravitationTargetEntry targetEntry in GravitationObjectResistry.Entries)
            {
                Vector3 targetPosition = targetEntry.target.transform.position;
                float Distance = Vector3.Distance(this.transform.position, targetPosition);

                if (Distance < CurrentAttractRange && Distance > 0.01f)
                {
                    if (targetEntry.target.TryGetComponent<IGravitationStatus>(out var targetData))
                    {
                        GravitationTargetStatusBlock targetStatusBlock = targetData.statusBlockGetter;

                        Vector3 AttractForce = AttractVectleCaluculate.CalculateAttractVectle(this.transform.position, targetPosition, CurrentPlanetMass, targetStatusBlock.Mass, Distance);
                        targetEntry.rigidbody.AddForce(AttractForce * targetStatusBlock.Mass, ForceMode.Force);
                    }
                }
            }
        }
    }
}
//    ※※※※※　　念のため関数保存　　※※※※
//Vector3 GetBlowVectle(GravitationTargetStatusBlock targetStatusBlock, Vector3 targetPos, float Dist)    //  吹き飛ばしベクトル取得
//{
//    Vector3 dir = (this.transform.position - targetPos).normalized;
//    float forceMag = (CurrentPlanetMass * planetStatusBlock.Mass) / (Dist * Dist);
//    Vector3 acceleration = forceMag * dir / targetStatusBlock.Mass;

//    return acceleration;
//}