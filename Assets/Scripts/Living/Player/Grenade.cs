using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour,IPooledObject
{
    [SerializeField]public float boomTime;
    [SerializeField]public float boomRadius;
    [SerializeField]public float boomDamage;
    public LayerMask boomLayerMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnObjectSpawn()
    {
        //特效
        Explode();
    }

    void Explode()
    {
        StartCoroutine(CExplode());
    }

    IEnumerator CExplode()
    {
        yield return new WaitForSeconds(boomTime);
        Collider[] colliders = Physics.OverlapSphere(transform.position,boomRadius,boomLayerMask);
        for (int i=0;i<colliders.Length;i++)
        {
            IDamageable damageable = colliders[i].GetComponent<IDamageable>();
            damageable?.TakeHit(boomDamage,transform.position,(colliders[i].transform.position - transform.position).normalized);
        }
        gameObject.SetActive(false);
    }
}
