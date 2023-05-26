using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private float maxHP = 100f;

    public event EventHandler OnDead;
    public event EventHandler OnDamaged;

    private float currentHP;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void Damage(float dmg)
    {
        currentHP = Mathf.Clamp(currentHP - dmg, 0, maxHP);

        OnDamaged?.Invoke(this, EventArgs.Empty);

        if(currentHP == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHeathNormalized() => currentHP / maxHP;
}
