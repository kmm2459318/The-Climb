using UnityEngine;
using TheClimb.Astral;
using TheClimb.Item;

namespace TheClimb.UniversalGravity
{
    public class GravitationTargetAttracter : MonoBehaviour    //  万有引力影響対象のオブジェクトを引き寄せるするスクリプト
    {
        [SerializeField] PlanetStatus planetStatus;         //  天体のステータス群
        GravitationStatusBlock gravitationStatusBlock;                //  天体のステータスクブロック

        public GravitationLevel CurrentGravitationLevel;    //  重力レベル

        float CurrentAttractRange;    //  現在の万有引力の影響半径
        float CurrentPlanetMass;    //  現在の万有引力の強さ

        void Awake()
        {
            gravitationStatusBlock = planetStatus.GetGraviatationStatus(PlanetIDs.Earth);    //  地球のステータス取得

            CurrentGravitationLevel = gravitationStatusBlock.gravitationLevel;

            CurrentPlanetMass = gravitationStatusBlock.Mass;
            CurrentAttractRange = gravitationStatusBlock.AttractRange;
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
                    if (targetEntry.target.TryGetComponent<IAtractable>(out var targetData))
                    {
                        GravitationTargetStatusBlock targetStatusBlock = targetData.statProperty;

                        if(targetStatusBlock.gravitationTargetTag == GravitationTargetTag.Item)
                        {
                            targetData.OnAttracting();
                        }
                        Vector3 AttractForce = AttractVectleCaluculate.CalculateAttractVectle(this.transform.position, targetPosition, CurrentPlanetMass, targetStatusBlock.Mass, Distance);
                        targetEntry.rigidbody.AddForce(AttractForce * targetStatusBlock.Mass, ForceMode.Force);
                    }
                }
            }
        }
    }
}