using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    [SerializeField] protected float speed = 10f;
    [SerializeField] protected float lifeTime = 2f;

    protected virtual void Start()
    {
        Destroy(gameObject, lifeTime );
    }

    public abstract void Shoot(Vector3 direction);
}
