using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTakeDetection : MonoBehaviour
{
    [SerializeField]public float damage;
    private void OnTriggerEnter(Collider other) {
        IDamageable damageable = other.GetComponent<IDamageable>();
        damageable?.TakeDamage(damage);
    }
}
