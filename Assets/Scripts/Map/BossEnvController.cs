using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//  using Unity.MLAgents;
using System;

public class BossEnvController : MonoBehaviour
{
    // [System.Serializable]
    // public class PlayerInfo
    // {
    //     public Boss Agent;
    //     [HideInInspector]
    //     public Vector3 StartingPos;
    //     [HideInInspector]
    //     public Quaternion StartingRot;
    //     [HideInInspector]
    //     public Rigidbody Rb;
    // }
    // [System.Serializable]
    // public class ChaserInfo
    // {
    //     public Chaser Agent;
    //     [HideInInspector]
    //     public Vector3 StartingPos;
    //     [HideInInspector]
    //     public Quaternion StartingRot;
    //     [HideInInspector]
    //     public Rigidbody Rb;
    // }
    // private int m_ResetTimer;
    
    // /// <summary>
    // /// Max Academy steps before this platform resets
    // /// </summary>
    // /// <returns></returns>
    // [Header("Max Environment Steps")] public int MaxEnvironmentSteps = 25000;
    
    // //List of Agents On Platform
    // public List<PlayerInfo> AgentsList = new List<PlayerInfo>();
    // public List<ChaserInfo> ChaserList = new List<ChaserInfo>();
    // // public List<Chaser> chaserList = new List<Chaser>();
    
    // private GameObject m_AAgent;
    // private GameObject m_BAgent;
    // public Transform floor;
    // public GameObject obsHolder;
    // public GameObject obstaclePrefab;
    // [SerializeField]public int minObsCount;
    // [SerializeField]public int maxObsCount;
    // List<Vector3> loactionList;
    // private SimpleMultiAgentGroup chaserGroup;
    // int alivePlayer,aliveChaser;
    
    // // Start is called before the first frame update
    // void Start()
    // {
    //     loactionList = new List<Vector3>();
    //     chaserGroup = new SimpleMultiAgentGroup();
    //     foreach (var item in AgentsList)
    //     {
    //         item.StartingPos = item.Agent.transform.position;
    //         item.StartingRot = item.Agent.transform.rotation;
    //         item.Rb = item.Agent.GetComponent<Rigidbody>();
    //     }
    //     foreach (var item in ChaserList)
    //     {
    //         item.StartingPos = item.Agent.transform.position;
    //         item.StartingRot = item.Agent.transform.rotation;
    //         item.Rb = item.Agent.GetComponent<Rigidbody>();
    //         chaserGroup.RegisterAgent(item.Agent);
    //     }
    //     alivePlayer = AgentsList.Count;
    //     aliveChaser = ChaserList.Count;
    //     ResetScene();
    // }
    // void FixedUpdate()
    // {
    //     m_ResetTimer += 1;
    //     if (m_ResetTimer >= MaxEnvironmentSteps && MaxEnvironmentSteps > 0 )
    //     {
    //         chaserGroup.GroupEpisodeInterrupted();
    //         ResetScene();
    //     }
    //     FallToDie();
    // }

    // private void FallToDie()
    // {
    //     foreach (var item in AgentsList)
    //     {
    //         if (item.Agent.transform.position.y <= transform.position.y - 1f)
    //         {
    //             item.Agent.Die();
    //             // ResetScene();
    //         }
    //     }
    // }

    // public void ResetScene()
    // {
    //     loactionList.Clear();
    //     m_ResetTimer = 0;

    //     //Reset Obstacles
    //     int obsChildCount = obsHolder.transform.childCount;
    //     for (int i=0;i<obsChildCount;i++)
    //     {
    //         Destroy(obsHolder.transform.GetChild(i).gameObject);
    //     }

    //     //Reset Agents
    //     loactionList = new List<Vector3>();
    //     foreach (var item in AgentsList)
    //     {
    //         var randomPosX = UnityEngine.Random.Range(-5f, 5f);
    //         var rot = item.Agent.rotSign * UnityEngine.Random.Range(80.0f, 100.0f);
    //         var newRot = Quaternion.Euler(0, rot, 0);
    //         item.Agent.transform.SetPositionAndRotation(item.StartingPos, newRot);

    //         item.Rb.velocity = Vector3.zero;
    //         item.Rb.angularVelocity = Vector3.zero;
    //         item.Agent.gameObject.SetActive(true);
    //         item.Agent.OnEpisodeBegin();
    //     }
    //     foreach (var item in ChaserList)
    //     {
    //         var randomPosX = UnityEngine.Random.Range(-5f, 5f);
    //         var rot = item.Agent.rotSign * UnityEngine.Random.Range(80.0f, 100.0f);
    //         var newRot = Quaternion.Euler(0, rot, 0);
    //         item.Agent.transform.SetPositionAndRotation(item.StartingPos, newRot);

    //         item.Rb.velocity = Vector3.zero;
    //         item.Rb.angularVelocity = Vector3.zero;
    //         item.Agent.gameObject.SetActive(true);
    //         item.Agent.OnEpisodeBegin();
    //     }
    //     alivePlayer = AgentsList.Count;
    //     aliveChaser = ChaserList.Count;
    //     //左平台
    //     int obsCount = UnityEngine.Random.Range(minObsCount,maxObsCount);
    //     for (int i=0;i<obsCount;i++)
    //     {
    //         GameObject obs = Instantiate(obstaclePrefab,new Vector3(UnityEngine.Random.Range(0,30) + 0.5f,0.5f,UnityEngine.Random.Range(0,30) + 0.5f) + floor.position,Quaternion.identity,obsHolder.transform);
    //         if (loactionList.Count > 0)
    //         {
    //             for (int j=0;j<loactionList.Count;j++)
    //             {
    //                 if (loactionList[j] == obs.transform.position)
    //                 {
    //                     break;
    //                 }
    //                 if (j == loactionList.Count - 1)
    //                 {
    //                     loactionList.Add(obs.transform.position);
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             loactionList.Add(obs.transform.position);
    //         }
    //     }
    //     // //右平台
    //     // for (int i=0;i<obsCount;i++)
    //     // {
    //     //     GameObject obs = Instantiate(obstaclePrefab,new Vector3(UnityEngine.Random.Range(0,30) + 0.5f + 10f,0.5f,UnityEngine.Random.Range(0,30) + 0.5f) + floor.position,Quaternion.identity,obsHolder.transform);
    //     //     if (loactionList.Count > 0)
    //     //     {
    //     //         for (int j=0;j<loactionList.Count;j++)
    //     //         {
    //     //             if (loactionList[j] == obs.transform.position)
    //     //             {
    //     //                 break;
    //     //             }
    //     //             if (j == loactionList.Count - 1)
    //     //             {
    //     //                 loactionList.Add(obs.transform.position);
    //     //             }
    //     //         }
    //     //     }
    //     //     else
    //     //     {
    //     //         loactionList.Add(obs.transform.position);
    //     //     }
    //     // }

    // }

    // private void Update() {
    //     if (Input.GetKey(KeyCode.L))
    //     {
    //         ResetScene();
    //     }
    // }

    // public void SomeoneHit()
    // {
    //     foreach (var agent in AgentsList)
    //     {
    //         agent.Agent.AddReward(-0.5f);
    //     }
    //     chaserGroup.AddGroupReward(1f);
    // }

    // public void PlayerDied()
    // {
    //     chaserGroup.AddGroupReward(2f);
    //     foreach(var item in AgentsList)
    //     {
    //         item.Agent.AddReward(-1f);
    //     }
    //     chaserGroup.EndGroupEpisode();
    //     if (--alivePlayer<=0)
    //     {
    //         ResetScene();
    //     }
    // }

    // public void ChaserDied()
    // {
    //     chaserGroup.EndGroupEpisode();
    //     if (--aliveChaser <= 0)
    //     {
    //         ResetScene();
    //     }
    // }

}
