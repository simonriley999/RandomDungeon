using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour,IPooledObject
{
    [SerializeField]public float life;
    [SerializeField]public float damage;
    [SerializeField]public float speed;
    [SerializeField]public float skinWidth;//增加子弹检测距离
    private float aliveTime;//记录子弹存活时间
    private Ray ray;
    private RaycastHit hitInfo;
    public LayerMask collisionLayerMask;//可以检测到的layer
    [SerializeField]public float startScanRadius;
    GunController playerGun;

    // Start is called before the first frame update
    private void Start() {
        if (playerGun != null)
        {
            playerGun = GameObject.FindGameObjectWithTag("Player").GetComponent<GunController>();
            playerGun.onGunChanged += DamageChange;
        }
    }
    public void OnObjectSpawn()
    {
        aliveTime = life;
        Collider[] initialCollisions = Physics.OverlapSphere(transform.position,(GetComponent<SphereCollider>().radius + startScanRadius) * transform.localScale.x,collisionLayerMask);
        if (initialCollisions.Length > 0)
        {
            HitEnemy(initialCollisions[0],transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        aliveTime -= Time.deltaTime;
        CheckCollision();
        AliveTimeUp();
    }

    void CheckCollision()
    {
        ray = new Ray(transform.position,transform.forward);
        if (Physics.Raycast(ray,out hitInfo,skinWidth + speed * Time.deltaTime,collisionLayerMask,QueryTriggerInteraction.Collide))
        {
            HitEnemy(hitInfo.collider,transform.position);
        }
    }

    private void HitEnemy(RaycastHit _hitInfo)//old
    {
        IDamageable damageable = _hitInfo.collider.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
        // Destroy(gameObject);
        aliveTime = life;
        gameObject.SetActive(false);
    }

    private void AliveTimeUp()
    {
        if (aliveTime<=0)
        {
            aliveTime = life;
            gameObject.SetActive(false);
        }
    }

    private void HitEnemy(Collider _c,Vector3 _hitPoint)
    {
        IDamageable damageable = _c.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeHit(damage,_hitPoint,transform.forward);
        }
        // Destroy(gameObject);
        aliveTime = life;
        gameObject.SetActive(false);
    }

    void DamageChange(float _damage)
    {
        damage = _damage;
    }

}
