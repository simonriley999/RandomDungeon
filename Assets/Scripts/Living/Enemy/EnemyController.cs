using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : LivingEntity
{
    public enum State
    {
        Idle,
        Chasing,
        Attacking
    }
    public State currentState;
    private NavMeshAgent nav;
    private Transform target;
    private GameObject player;
    private LivingEntity targetEntity;
    [SerializeField]public float moveSpeed;
    [SerializeField]public float attackRate;
    [SerializeField]public float upadateRate;
    [SerializeField]public float attackAnimationSpeed;
    [SerializeField]public float attackDistance;
    [SerializeField]public float damage;
    private float nextAttackTime;
    private Color idleColor;
    private float collisionRadius,targetCollisionRadius;
    private bool hasTarget;
    private float playTime;
    [Header("UI")]
    public RectTransform healthHolder;
    public RectTransform healthRect;
    public Vector2 healthOffset;
    public RectTransform healthShadow;
    public float shadowEffectSpeed;
    protected override void Start()
    {
        base.Start();
        currentState = State.Chasing;
        nav = GetComponent<NavMeshAgent>();
        nav.speed = moveSpeed;
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            hasTarget = true;
            target = player.transform;
            targetEntity = target.GetComponent<LivingEntity>();
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
            targetEntity.onDeath += targetDeath;
        }
        collisionRadius = GetComponent<CapsuleCollider>().radius;
        nextAttackTime = Time.time;
        idleColor = GetComponent<Renderer>().material.color;
        Utilities.UIFollowCharacter(healthHolder,transform.position,healthOffset);
        StartCoroutine(UpdatePath());
    }

    // Update is called once per frame
    void Update()
    {
        CheckToAttack();
        Utilities.UIFollowCharacter(healthHolder,transform.position,healthOffset);
        // nav.SetDestination(target.position);//一直追到死类型
    }

    IEnumerator UpdatePath()
    {
        while(hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPos = target.position - dirToTarget * (targetCollisionRadius + attackDistance/2);
                if (!isDead)
                {
                    
                    nav.SetDestination(targetPos);
                }
            }
            yield return new WaitForSeconds(upadateRate);
        }
    }

    void CheckToAttack()
    {
        if (Time.time > nextAttackTime && hasTarget)
        {
            float sqrtDstToTarget = (target.position - transform.position).sqrMagnitude;
            if (sqrtDstToTarget < Mathf.Pow(attackDistance,2))
            {
                nextAttackTime = attackRate + Time.time;
                StartCoroutine(Attack());
            }
        }
    }
    
    IEnumerator Attack()
    {
        currentState = State.Attacking;
        nav.enabled = false;
        Material material = GetComponent<Renderer>().material;
        material.color = Color.red;
        Vector3 originalPos = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPos = target.position - dirToTarget * (targetCollisionRadius);
        float percent = 0;
        bool hasAppliedDamage = false;
        while (percent <= 1)
        {
            if (!hasAppliedDamage && percent >= 0.5f)
            {
                hasAppliedDamage = true;
                targetEntity.GetComponent<IDamageable>().TakeDamage(damage);
            }
            percent += Time.deltaTime * attackAnimationSpeed;
            float interploation = 4*(-percent*percent + percent);//先快后慢动画，欲知原因请画出函数图像
            transform.position = Vector3.Lerp(originalPos,attackPos,interploation);
            yield return null;
        }
        material.color = idleColor;
        currentState = State.Chasing;
        nav.enabled = true;
        yield return null;
    }

    void targetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    public override void TakeHit(float _damageAmount, Vector3 _hitPoint, Vector3 _hitDirection)
    {
        if (_damageAmount >= health)
        {
            GameObject p = ObjectPooler.instance.SpawnFromPool("DeathEffect",_hitPoint,Quaternion.FromToRotation(Vector3.forward,_hitDirection));
            p.GetComponent<ParticleSystem>().Play();
            playTime = p.GetComponent<ParticleSystem>().startLifetime;
            StartCoroutine(TimeToSetActiveFalse(p));
        }
        base.TakeHit(_damageAmount, _hitPoint, _hitDirection);
        //UI
        healthRect.localScale = new Vector3(health/maxHealth, 1 ,1);
        StartCoroutine(HealthShadowEffect());
    }

    IEnumerator HealthShadowEffect()
    {
        float shadowX = healthShadow.localScale.x;
        while(shadowX >= healthRect.localScale.x)
        {
            shadowX -= shadowEffectSpeed * Time.deltaTime;
            healthShadow.localScale = new Vector3(shadowX,1,1);
            yield return null;
        }
    }

    IEnumerator TimeToSetActiveFalse(GameObject _p)
    {
        while (true)
        {
            if ((playTime-=Time.deltaTime) <= 0)
            {
                Debug.Log("isEmitting == false");
                _p.SetActive(false);
                break;
            }
            yield return null;
        }
        yield return null;
    }

}
