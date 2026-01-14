using UnityEngine;

public class Bomb_Aim : MonoBehaviour
{
    [SerializeField] private Transform p_leftPos;
    [SerializeField] private Transform p_rightPos;
    [SerializeField] private PlayerMove p_move;
    [SerializeField] private float last_Direction = 0;

    public  void UpdateDirection(float moveInput)
    {    //プレイヤーがどっちを向いたか
         if (p_move.MoveInput < 0) last_Direction = -1;
         else if (p_move.MoveInput > 0) last_Direction = 1;
    }

    public Transform GetSpawnPos()
    {
        return (last_Direction < 0 ? p_leftPos : p_rightPos);
    }

    public Vector3 GetShootDirection()
    {
        Transform pos = GetSpawnPos();

        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 dir = (worldPos - pos.position).normalized;

        float angle = Mathf.Atan2(dir.y ,dir.x) * Mathf.Rad2Deg;
        pos.rotation = Quaternion.Euler(0,0,angle);

        return pos.right;

    }
}
