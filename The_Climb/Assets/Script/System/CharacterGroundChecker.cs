using UnityEngine;

//  キャラクター接地判定確認
public class CharacterGroundChecker : MonoBehaviour
{
    [SerializeField] float GroundCheckDis;

    public float GroundCheckDisProperty => GroundCheckDis;
    //  接地判定
    public bool CheckIsGround(Vector3? position = null)
    {
        Vector3 CheckPos = position ?? this.transform.position;
        return Physics.Raycast(CheckPos, Vector3.down, GroundCheckDis, GameLayer.ToMask(GameLayers.GROUND));
    }
}
