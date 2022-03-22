using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;//委托

public class LivingEntity : MonoBehaviour,IDamageable
{
    public float maxHealth;
    protected float health;
    protected bool isDead;
    protected bool isUndamageable;
    public event Action<float,float> onHealthChange; 
    public event Action onDeath;

    protected virtual void Start() {
        health = maxHealth;
        isUndamageable = false;
    }

    protected void Die()
    {
        if (!isDead)
        {
            isDead = true;
            if (onDeath != null)
            {
                onDeath();
            }
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float _damageAmount)
    {
        if (!isUndamageable)
        {
            health -= _damageAmount;
            if (onHealthChange != null)
            {
                onHealthChange(health,maxHealth);
            }
            if (health <= 0)
            {
                Die();
            }
        }
        }
        

    public virtual void TakeHit(float _damageAmount,Vector3 _hitPoint,Vector3 _hitDirection)
    {
        //
        TakeDamage(_damageAmount);
    }
}
