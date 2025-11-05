using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected Transform FirePos;
    [SerializeField] protected GameObject BulletPrefab;
    [SerializeField] protected float FireRate = 0.2f;
    protected float NextFireTime;

    public abstract void Fire(Vector3 Direction);
}
