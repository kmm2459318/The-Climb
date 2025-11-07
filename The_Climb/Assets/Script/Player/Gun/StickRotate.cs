//using UnityEngine;

//public class StickRotate : MonoBehaviour
//{
//    void Update()
//    {
//        if (Input.GetMouseButton(0))
//        {
//            //Debug.Log("クリックされました");
//            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//            worldMouse.z = 0;

//            Vector3 direction = mousePos - transform.position;
//            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
//            Debug.Log(Quaternion.Euler(0,0, angle));
//            transform.rotation = Quaternion.Euler(0, 0, angle);
//        }
//    }
//}
