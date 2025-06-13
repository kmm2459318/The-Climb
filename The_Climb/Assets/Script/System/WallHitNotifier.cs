using UnityEngine;
//  �ǂɓ����������ǂ������肷��
public class WallHitNotifier : MonoBehaviour
{
    IWallHitTable WallHitTable;    //  �ǂɓ����������̃C���^�[�t�F�C�X�̃C���X�^���X
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
                //  ���̃X�N���v�g�����Ă���I�u�W�F�N�g�̕ǂɓ����������̏���
                WallHitTable.OnHitWall();
            }
        }
    }
}
