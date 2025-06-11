using UnityEngine;

public class CharacterGroundChecker : MonoBehaviour  //  �L�����N�^�[�ڒn����m�F
{
    [SerializeField] float GroundCheckDis;
    //  �ڒn����
    public bool CheckIsGround()
    {
        return Physics.Raycast(this.transform.position, Vector3.down, GroundCheckDis, GameLayer.ToMask(GameLayers.GROUND));
    }
}
