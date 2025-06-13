using UnityEngine;
//  壁に当たったかどうか判定する
public class WallHitNotifier : MonoBehaviour
{
    IWallHitTable WallHitTable;    //  壁に当たった時のインターフェイスのインスタンス
    void Awake()
    {
        WallHitTable = GetComponent<IWallHitTable>();
    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(this.gameObject.name);
        if(collision.gameObject.CompareTag(TagName.Wall))
        {
            if(WallHitTable != null)
            {
                //  このスクリプトがついているオブジェクトの壁に当たった時の処理
                WallHitTable.OnHitWall();
            }
        }
    }
}
