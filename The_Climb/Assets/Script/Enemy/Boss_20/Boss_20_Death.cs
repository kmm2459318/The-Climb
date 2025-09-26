using UnityEngine;

public class Boss_20_Death : MonoBehaviour
{
    private int Count = 0;
    public Boss_20_StatusObjectScript status;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") )
        {
            Count++;
            if (Count >= status.HP) Die();         
        }
    }
    private void Die()
   {
        Destroy(gameObject); // ƒ{ƒX©g‚ğ”j‰ó
    }
  
}
