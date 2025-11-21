using UnityEngine;
using UnityEngine.Playables;

public class KidnapBuddy : MonoBehaviour
{
    [HideInInspector] public StalkerHandController handController;

    private void Start()
    {
        handController = transform.parent.gameObject.GetComponent<StalkerHandController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!handController.buddyController.beingKidnapped && !handController.isKidnapping)
        {
            if (other.CompareTag("Buddy") && !handController.playerState.carryingBuddy)  //Buddyが孤立してる場合
            {
                handController.BuddyGet();
            }
            else if (other.CompareTag("Player") && handController.playerState.carryingBuddy)  //Buddyをおんぶしてる場合
            {
                PlayerMind playerMind = other.GetComponent<PlayerMind>();

                //敵とプレイヤーの位置でノックバックの方向を決める
                int dir = handController.mainStalker.transform.position.x - other.gameObject.transform.position.x <= 0 ? 1 : -1;
                handController.playerKnock.DoKnockBack(dir); //ノックバック
                handController.BuddyGet();
                playerMind.SanityDecreaseEvent(5);
            }
        }
    }
}
