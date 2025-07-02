using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    PlayerState state;
    PlayerJump jump;
    PlayerSpecialAction special;

    private float headingSafeTime = 0.1f;
    private float headingSafeCounter = 0f;

    void Start()
    {
        state = gameObject.transform.parent.gameObject.GetComponent<PlayerState>();
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

        if (this.gameObject.name == "HeadingAttack")
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
        else if (this.gameObject.name == "MeteorDropAttack")
        {
            MeteorDropFalse();
        }
    }

    private void HeadingFalse()
    {
        this.gameObject.SetActive(false);
        //Debug.Log("false後" + special.headingAttack);
    }

    private void MeteorDropFalse()
    {
        if (!special.meteorDrop)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void PlayerYMoveReset()
    {
        state.RigidBody.linearVelocity = new Vector3(state.RigidBody.linearVelocity.x, 0, state.RigidBody.linearVelocity.z);
        jump.jumping = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.gameObject.name == "HeadingAttack" && !other.gameObject.CompareTag("SearchItem"))
        {
            //ぶつかったときの突っかかりを消す
            PlayerYMoveReset();
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            //敵を消す
            Destroy(other.gameObject);
        }
        
        if (other.gameObject.CompareTag("BreakBlock") && (special.highJumpUsed || special.meteorDrop))
        {
            //ブロックを消す
            Destroy(other.gameObject);
        }
    }
}
