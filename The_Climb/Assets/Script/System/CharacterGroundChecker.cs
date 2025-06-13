using UnityEngine;

//  �L�����N�^�[�ڒn����m�F
public class CharacterGroundChecker : MonoBehaviour
{
    [SerializeField] float GroundCheckDis;

    public float GroundCheckDisProperty => GroundCheckDis;
    //  �ڒn����
    public bool CheckIsGround(Vector3? position = null)
    {
        Vector3 CheckPos = position ?? this.transform.position;
        return Physics.Raycast(CheckPos, Vector3.down, GroundCheckDis, GameLayer.ToMask(GameLayers.GROUND));
    }
}
