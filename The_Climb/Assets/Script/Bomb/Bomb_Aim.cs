using UnityEngine;

public class Bomb_Aim : MonoBehaviour
{
    [SerializeField] private Camera aimCamera; 
    [SerializeField] private Transform player;
    [SerializeField] private Transform p_leftPos;
    [SerializeField] private Transform p_rightPos;
    [SerializeField] private float last_Direction = 0;

    public  void UpdateDirection(float moveInput)
    {    //プレイヤーがどっちを向いたか
         if (moveInput < 0) last_Direction = -1;
         else if (moveInput > 0) last_Direction = 1;
    }

    public Transform GetSpawnPos()
    {
        return (last_Direction < 0 ? p_leftPos : p_rightPos);
    }

    public Vector3 GetShootDirection()
    {
        Transform pos = GetSpawnPos();

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = aimCamera.WorldToScreenPoint(pos.position).z;
        Vector3 worldPos = aimCamera.ScreenToWorldPoint(mousePos);
        Vector3 dir = (worldPos - player.position).normalized;

        float angle = Mathf.Atan2(dir.y ,dir.x) * Mathf.Rad2Deg;
        pos.rotation = Quaternion.LookRotation(dir); 
        
        return dir;

    }
}
