
using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Transform WeaponPos;     //武器の場所
    [SerializeField] private Camera MainCamera;


    private void Update()
    {
        AimMouse();
    }

    //マウスで出る方向の指定
    private void AimMouse()
    {
        Vector3 MousePos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 Direction = (MousePos - WeaponPos.position).normalized;

        //マウスの角度を計算
        float Angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
        WeaponPos.rotation = Quaternion.Euler(0, 0, Angle);

    }
}
