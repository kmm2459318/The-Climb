using UnityEngine;

public class JumpToggleBlock : MonoBehaviour
{
    [SerializeField] private GameObject[] BlockGroupA;
    [SerializeField] private GameObject[] BlockGroupB;

    private PlayerJump PlayerJump;
    private bool IsBlockAActive = true; //BlockAがアクティブかどうかを記録するフラグ

    private void Awake()
    {
        //シーン上からそれぞれのタグを持つオブジェクトを探して配列に格納
        BlockGroupA = GameObject.FindGameObjectsWithTag("BlockA");
        BlockGroupB = GameObject.FindGameObjectsWithTag("BlockB");

        PlayerJump = FindObjectOfType<PlayerJump>();
    }

    private void OnEnable()
    {
        //プレイヤーがジャンプしたときにToggleBlockを実行するように登録
        PlayerJump.OnJumped += ToggleBlocks;
    }

    private void OnDisable()
    {
        //無効化時にイベントを解除(エラー防止)
        PlayerJump.OnJumped -= ToggleBlocks;
    }

    //ブロック表示を切り替える
    private void ToggleBlocks()
    {
        //フラグを反転させる
        IsBlockAActive = !IsBlockAActive;

        //BlockGroupAのブロックを一括で切り替え
        foreach (var block in BlockGroupA)
        {
            if (block != null) block.SetActive(IsBlockAActive);
        }
        
        //BlockGroupBのブロックを一括で切り替え(Aと逆にする)
        foreach (var block in BlockGroupB)
        {
            if (block != null) block.SetActive(!IsBlockAActive);
        }
    }
}
