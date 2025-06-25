using UnityEngine;

/// <summary>
/// プレイヤーがこのオブジェクトに一定距離まで近づいたら、
/// StartHole なら前の階へ、GoalHole なら次の階へ切り替える。
/// </summary>
public class FloorTransitionPortal : MonoBehaviour
{
    public enum PortalType
    {
        StartHole,
        GoalHole
    }

    [Header("ポータルの種類")]
    public PortalType portalType;

    [Header("距離がこの値以下になったら反応")]
    public float triggerDistance = 0.25f;

    [Header("フロア管理への参照")]
    public FloorManager floorManager;

    [Header("プレイヤーへの参照")]
    public Transform player;

    private bool hasTriggered = false;

    void Update()
    {
        if (hasTriggered || player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= triggerDistance)
        {
            hasTriggered = true;

            if (portalType == PortalType.GoalHole)
            {
                floorManager.MoveToNextFloor();
            }
            else if (portalType == PortalType.StartHole)
            {
                floorManager.MoveToPreviousFloor();
            }
        }
    }
}
