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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground") && this.gameObject.name == "HeadingAttack")
        {
            Debug.Log("headingAttack");
            state.RigidBody.linearVelocity = new Vector3(state.RigidBody.linearVelocity.x, 0, state.RigidBody.linearVelocity.z);
            special.highJumpStop = true;
            jump.jumping = false;
        }

        if (other.gameObject.CompareTag("Ground") && this.gameObject.name == "MeteorDropAttack")
        {
            Debug.Log("Break");
        }
    }
}
