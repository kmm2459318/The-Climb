using UnityEngine;

public class CharacterGroundChecker : MonoBehaviour  //  キャラクター接地判定確認
{
    [SerializeField] float GroundCheckDis;
    //  接地判定
    public bool CheckIsGround()
    {
        return Physics.Raycast(this.transform.position, Vector3.down, GroundCheckDis, GameLayer.ToMask(GameLayers.GROUND));
    }
}
