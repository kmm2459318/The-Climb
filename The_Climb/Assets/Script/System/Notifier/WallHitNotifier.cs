using UnityEngine;

//  •Ç‚Ö‚ÌÕ“Ë‚ğ’Ê’m
public class WallHitNotifier : CollisionNotifier<IWallHitTable>
{
    void OnCollisionEnter(Collision collision)
    {
        //  •Ç‚É“–‚½‚Á‚½‚Ìˆ—‚ğÀs
        NotifyIfTagMatches(collision, TagName.Wall, h => h.OnHitWall());
    }
}
