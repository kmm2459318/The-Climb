using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    PlayerState state;
    PlayerMove move;
    PlayerJump jump;
    PlayerSpecialAction special;

    private float headingSafeTime = 0.1f;
    private float headingSafeCounter = 0f;

    void Start()
    {
        state = gameObject.transform.parent.gameObject.GetComponent<PlayerState>();
        move = gameObject.transform.parent.gameObject.GetComponent<PlayerMove>();
        jump = gameObject.transform.parent.gameObject.GetComponent<PlayerJump>();
        special = gameObject.transform.parent.gameObject.GetComponent<PlayerSpecialAction>();
    }

    void Update()
    {
        if (jump.jumpCoolActive)
        {
            headingSafeCounter = headingSafeTime;  //ジャンプ開始時にリセット
        }
        else if (headingSafeCounter > 0f)
        {
            headingSafeCounter -= Time.deltaTime;
        }

        //各判定を終了させる
        if (gameObject.name == "HeadingAttack")
        {
            //Debug.Log(state.RigidBody.linearVelocity.y);
            if (headingSafeCounter <= 0f &&
                state.RigidBody.linearVelocity.y < 0.5f)
            { 
                HeadingFalse();
                headingSafeCounter = headingSafeTime;
                special.highJumpUsed = false;
            }
        }
        else if (gameObject.name == "MeteorDropAttack")
        {
            MeteorDropFalse();
        }
        else if (gameObject.name == "QuickJumpAttack")
        {
            QuickJumpFalse();
        }
    }

    private void HeadingFalse()
    {
        gameObject.SetActive(false);
        //Debug.Log("false後" + special.headingAttack);
    }

    private void MeteorDropFalse()
    {
        if (!special.meteorDrop)
        {
            gameObject.SetActive(false);
        }
    }

    private void QuickJumpFalse()
    {
        if (move.maxAirSpeed < 12f)
        {
            gameObject.SetActive(false);
        }
    }

    private void PlayerYMoveReset()
    {
        state.RigidBody.linearVelocity = new Vector3(state.RigidBody.linearVelocity.x, 0, state.RigidBody.linearVelocity.z);
        jump.jumping = false;
    }

    private IEnumerator HitStop()
    {
        Vector3 PlayerVelocity = state.RigidBody.linearVelocity;
        for (int i = 0; i <= 3; i++)
        {
            //Debug.Log("stop");
            state.RigidBody.linearVelocity = Vector3.zero;
            yield return null;
        }

        state.RigidBody.linearVelocity = PlayerVelocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.name == "HeadingAttack" && !other.gameObject.CompareTag("SearchItem") && !special.highJumpUsed)
        {
            //ぶつかったときの突っかかりを消す
            PlayerYMoveReset();
        }

        //敵に当たったら
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("BreakBlock") && (special.highJumpUsed || special.meteorDrop))
        {
            //敵を消す
            Destroy(other.gameObject);
            StartCoroutine(HitStop());
        }
    }
}
