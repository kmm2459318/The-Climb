using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    PlayerState state;
    PlayerJump jump;
    PlayerSpecialAction special;

    void Start()
    {
        state = gameObject.transform.parent.gameObject.GetComponent<PlayerState>();
        jump = gameObject.transform.parent.gameObject.GetComponent<PlayerJump>();
        special = gameObject.transform.parent.gameObject.GetComponent<PlayerSpecialAction>();
    }

    void Update()
    {
        if (this.gameObject.name == "HeadingAttack")
        {
            HeadingFalse();
        }
        else if (this.gameObject.name == "MeteorDropAttack")
        {
            MeteorDropFalse();
        }
    }

    private void HeadingFalse()
    {
        if (state.RigidBody.linearVelocity.y < 1f && !jump.jumpCoolActive)
        {
            Debug.Log("false"+special.headingAttack);
            this.gameObject.SetActive(false);
        }
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
        if (other.gameObject.CompareTag("Untagged") && this.gameObject.name == "HeadingAttack")
        {
            Debug.Log("headingAttack");
        }

        if (other.gameObject.CompareTag("Untagged") && this.gameObject.name == "MeteorDropAttack")
        {
            Debug.Log("Break");
        }
    }
}
