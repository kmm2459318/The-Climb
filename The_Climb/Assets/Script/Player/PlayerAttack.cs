using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Untagged"))
        {
            Debug.Log("headingAttack");
        }

        if (other.gameObject.CompareTag("BreakBlock"))
        {
            Debug.Log("Break");
        }
    }
}
