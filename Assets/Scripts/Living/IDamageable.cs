using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float _damageAmount);
    void TakeHit(float _damageAmount,Vector3 _hitPoint,Vector3 _hitDirection);
}
