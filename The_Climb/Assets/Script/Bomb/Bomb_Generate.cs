using UnityEngine;

public class Bomb_Generate : MonoBehaviour
{
    [SerializeField] GameObject bombPrefab;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            GameObject bomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
        }
    }
}
