using UnityEngine;

//  “G‚ÆÕ“Ë‚µ‚½‚±‚Æ‚ğ’Ê’m
public class CollideEnemyNotifier : CollisionNotifier<ICollideEnemy>
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody == null)
        {
            return;
        }
        //  •Ç‚É“–‚½‚Á‚½‚Ìˆ—‚ğÀs
        NotifyIfTagMatches(collision, TagName.Enemy, h => h.OnCollideEnemy());
    }
}
