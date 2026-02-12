using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 3;
    public int currentHP;

    public bool isInvincible = false;

    public event Action OnDead;

    void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (currentHP <= 0) return;
        if (isInvincible) return;

        currentHP -= damage;

        if (currentHP <= 0)
        {
            currentHP = 0;
            OnDead?.Invoke();
        }
    }

    public void ResetHealth()
    {
        currentHP = maxHP;
        isInvincible = false;
    }
}