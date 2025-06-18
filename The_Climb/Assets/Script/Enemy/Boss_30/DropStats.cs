using UnityEngine;

[CreateAssetMenu(menuName = "EnemyStats/DropStats")]
public class DropStats : ScriptableObject
{
    public float rushSpeed = 10f;//往復速度
    public int diagonalRushCount = 3;//往復回数（片道ずつのカウント）
    public float meteorDropSpeed = 20f;//メテオドロップの速度
    public int meteorDropCount = 2;
    public float riseSpeed = 5f;
    public float waitBeforeMeteor = 1.5f;

    public float pointA_X = -10f;  // 左端（A地点）
    public float pointB_X = 10f;   // 右端（B地点）
    public float groundY = 0f;     // 地面Y座標（接地判定や落下停止用）
    public float hoverHeight = 20f; // メテオ前のホバーポジションY

    public float aimMoveLeftX = -3f;  // 狙い動作の左端
    public float aimMoveRightX = 3f;  // 狙い動作の右端
    public float aimMoveSpeed = 5f;   // 狙い動作のスピード
    public int aimMoveCount = 3;      // 往復回数（片道 = 1 回）
}
