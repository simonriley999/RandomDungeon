using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : LivingEntity
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
    public Animator animator;
    private LivingEntity targetEntity;
    [SerializeField]public float moveSpeed;
    [SerializeField]public float movementInterval;//每个动作间的间隔
    [SerializeField]public float movementAfterAngryIncrement;//暴怒每个动作间的间隔
    [SerializeField]public float callingTime;//请求援军动作时间
    [SerializeField]public int AssistanceAmount;
    [SerializeField]public float shootTime;
    [SerializeField]public float attackDistance;
    bool isCouldMeleeAttack;
    public Color angryColor;
    bool isAngry;
    public Color callingColor;
    private float collisionRadius,targetCollisionRadius;
    private bool hasTarget;
    public Gun equipedGun;
    public Sword sword;
    public GameObject swordBody;
    public Spawner spawner;
    [Header("UI")]
    public RectTransform healthHolder;
    public RectTransform healthRect;
    public RectTransform healthShadow;
    public float shadowEffectSpeed;

    protected override void Start() {
        base.Start();
        currentState = State.Idle;
        isAngry = false;
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

        if (sword != null)
        {
            sword.onMeleeAttackOver += SetChasing;
        }
        
    }

    private void Update() {
        if (hasTarget && currentState == State.Idle)
        {
            if (!isAngry && health <= maxHealth / 2)
            {
                isAngry = true;
                StartCoroutine(Angry());
            }

            int movement = UnityEngine.Random.Range(0,3);
            switch (movement)
            {
                case 0:
                    StartCoroutine(MeleeAttack());
                    break;
                case 1:
                    StartCoroutine(ShootAttack());
                    break;
                case 2:
                    StartCoroutine(CallAssistance());
                    break;
                default:
                    break;
            }
        }
    }
    IEnumerator UpdatePath()
    {
        if (hasTarget)
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
            yield return null;
            // yield return new WaitForSeconds(movementInterval);
        }
    }

    IEnumerator MeleeAttack()
    {
        currentState = State.Chasing;
        Debug.Log("MelleAttack");
        nav.Warp(transform.position);
        StartCoroutine(UpdatePath());
        isCouldMeleeAttack = true;
        yield return new WaitForEndOfFrame();
        while (true)
        {
            if (isCouldMeleeAttack && !nav.pathPending && nav.remainingDistance < 1f)//是否可以攻击
            {
                currentState = State.Attacking;
                // sword.SwingSword();
                animator.SetTrigger("MeleeAttack");
                isCouldMeleeAttack = false;
                break;
            }
            yield return null;
        }
        
        yield return new WaitForSeconds(movementInterval);
        currentState = State.Idle;
    }

    IEnumerator ShootAttack()
    {
        if (equipedGun != null)
        {
            Debug.Log("Shoot");
            equipedGun.gameObject.SetActive(true);
            currentState  = State.Attacking;
            float hasShootTime = 0f;
            while (hasShootTime < shootTime)
            {
                transform.LookAt(target.position);
                equipedGun.onTriggerHold();
                hasShootTime += Time.deltaTime;
                yield return null;
            }
            equipedGun.remainingBulletInMag = 100;
            yield return new WaitForSeconds(movementInterval);
            equipedGun.gameObject.SetActive(false);
            currentState = State.Idle;
        }
    }

    IEnumerator CallAssistance()
    {
        currentState = State.Attacking;
        isUndamageable = true;
        if (spawner != null)
        {
            animator.SetTrigger("CallAssistance");
            Debug.Log("CallAssistance");
            spawner.SpawnAssistance(AssistanceAmount);
        }
        yield return new WaitForSeconds(callingTime);
        isUndamageable = false;
        yield return new WaitForSeconds(movementInterval);
        currentState = State.Idle;
    }

    IEnumerator Angry()
    {
        currentState = State.Attacking;
        isUndamageable = true;
        animator.SetTrigger("Angry");
        Debug.Log("Angry");
        movementInterval -= movementAfterAngryIncrement;
        yield return new WaitForSeconds(1f);
        isUndamageable = false;
        currentState = State.Idle;
    }
    public override void TakeHit(float _damageAmount, Vector3 _hitPoint, Vector3 _hitDirection)
    {
        if (_damageAmount >= health)
        {
            GameObject p = ObjectPooler.instance.SpawnFromPool("DeathEffect",_hitPoint,Quaternion.FromToRotation(Vector3.forward,_hitDirection));
            p.GetComponent<ParticleSystem>().Play();
        }
        base.TakeHit(_damageAmount, _hitPoint, _hitDirection);
        //UI
        healthRect.localScale = new Vector3(health/maxHealth, 1 ,1);
        Debug.Log(healthRect.localScale.x);
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
    
    void targetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    public void SetChasing()
    {
        currentState = State.Chasing;
    }

}
