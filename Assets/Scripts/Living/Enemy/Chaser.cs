using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Actuators; */

public class Chaser// : Agent
{
    // [SerializeField]public float maxHealth;
    // public float health;
    // public enum Team
    // {
    //     Blue = 1,
    //     Red = 0
    // }
    // public Team currentTeam;
    // [SerializeField]public float moveSpeed;
    // [SerializeField]public float damage;
    // [SerializeField]public float attackInterval;
    // float nextAttackTime;
    // public BehaviorParameters m_BehaviorParameters;
    // EnvironmentParameters m_ResetParams;
    // public Rigidbody rigidbody;
    // public Transform target;
    // public float rotSign;
    // public BossEnvController bossEnvController;
    // public event Action onDeath;
    // bool isDead;
    

    // // Update is called once per frame
    // public override void Initialize()
    // {
    //     health = maxHealth;
    //     rotSign = 1f;
    //     isDead = false;
    //     m_BehaviorParameters = gameObject.GetComponent<BehaviorParameters>();
    //     if (m_BehaviorParameters.TeamId == (int)Team.Blue)
    //     {
    //         currentTeam = Team.Blue;
    //     }
    //     else
    //     {
    //         currentTeam = Team.Red;
    //     }
    //     rigidbody = GetComponent<Rigidbody>();
    //     rigidbody.maxAngularVelocity = 500;

    //     m_ResetParams = Academy.Instance.EnvironmentParameters;

    // }

    // public void Die()
    // {
    //     if (!isDead)
    //     {
    //         isDead = true;
    //         if (onDeath != null)
    //         {
    //             onDeath();
    //         }
    //         gameObject.SetActive(false);
    //         if (bossEnvController != null)
    //         {
    //             AddReward(-1f);
    //             // bossEnvController.ResetScene();
    //             bossEnvController.ChaserDied();
    //         }
    //     }
    // }

    // public void TakeDamage(float _damageAmount)
    // {
    //     health -= _damageAmount;

    //     if (health <= 0)
    //     {
    //         Die();
    //     }
    // }

    // public void TakeHit(float _damageAmount, Vector3 _hitPoint, Vector3 _hitDirection)
    // {
    //     TakeDamage(_damageAmount);
    //     return;
    // }

    // public override void OnEpisodeBegin()
    // {
    //     health = maxHealth;
    //     isDead = false;
    //     nextAttackTime = Time.time;
    // }

    // public override void CollectObservations(VectorSensor sensor)
    // {
    //     sensor.AddObservation(health);
    //     sensor.AddObservation(transform.position);
    // }

    // public override void OnActionReceived(ActionBuffers actions)
    // {
    //     var dirToGo = Vector3.zero;
    //     var rotateDir = Vector3.zero;

    //     var action = actions.DiscreteActions[0];

    //     switch (action)
    //     {
    //         case 1:
    //             dirToGo = transform.forward * 1f;
    //             break;
    //         case 2:
    //             dirToGo = transform.forward * -1f;
    //             break;
    //         case 3:
    //             dirToGo = transform.right * -0.75f;
    //             break;
    //         case 4:
    //             dirToGo = transform.right * 0.75f;
    //             break;
    //         case 5:
                
    //             break;
    //         case 6:
    //             rotateDir = transform.up * 1f;
    //             break;
    //         case 7:
    //             rotateDir = transform.up * -1f;
    //             break;
    //     }
    //     transform.Rotate(rotateDir, Time.fixedDeltaTime * 200f);
    //     rigidbody.velocity = dirToGo * moveSpeed;
    //     // rigidbody.AddForce(dirToGo * moveSpeed,
    //     //     ForceMode.VelocityChange);

    //     // if (transform.position.y <= -1f)
    //     // {
    //     //     Die();
    //     // }
    //     AddReward(-0.0001f);
    // }

    // public override void Heuristic(in ActionBuffers actionsOut)
    // {
    //     var discreteActionsOut = actionsOut.DiscreteActions;
    //     if (Input.GetKey(KeyCode.D))
    //     {
    //         discreteActionsOut[0] = 4;
    //     }
    //     else if (Input.GetKey(KeyCode.W))
    //     {
    //         discreteActionsOut[0] = 1;
    //     }
    //     else if (Input.GetKey(KeyCode.A))
    //     {
    //         discreteActionsOut[0] = 3;
    //     }
    //     else if (Input.GetKey(KeyCode.S))
    //     {
    //         discreteActionsOut[0] = 2;
    //     }
    //     else if (Input.GetKey(KeyCode.Q))
    //     {
    //         discreteActionsOut[0] = 6;
    //     }
    //     else if (Input.GetKey(KeyCode.E))
    //     {
    //         discreteActionsOut[0] = 7;
    //     }
    //     else if (Input.GetKey(KeyCode.Space))
    //     {
    //         discreteActionsOut[0] = 5;
    //     }
    // }

    // private void OnTriggerStay(Collider other) {
    //     Debug.Log(other.tag);
    //     if (Time.time > nextAttackTime && other.tag == "Player")
    //     {
    //         Debug.Log("Attack");
    //         IDamageable damageable = other.GetComponent<IDamageable>();
    //         if (bossEnvController != null)
    //         {
    //             bossEnvController.SomeoneHit();
    //         }
    //         damageable?.TakeDamage(damage);
    //         nextAttackTime = Time.time + attackInterval;
    //     }
    // }


}
