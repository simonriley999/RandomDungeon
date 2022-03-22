using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GrenadeController : MonoBehaviour
{
    [Header("Generation")]
    public GameObject grenadePrefab;
    [SerializeField]public float height;
    [SerializeField]public int resolution;//取得路径点的数量
    [SerializeField]public float flyTime;
    public Transform starPos;
    Vector3 endPos;
    bool isHolding;
    Vector3[] path;
    public LineRenderer lineRenderer;

    [Header("Paramater")]
    [SerializeField]public int grenadeAmount;
    // Start is called before the first frame update
    void Start()
    {
        path = new Vector3[resolution];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            isHolding = true;
            StartCoroutine(ThrowPath());
        }
        else if (Input.GetKeyUp(KeyCode.G))
        {
            isHolding = false;
            if (grenadeAmount > 0)
            {
                Throw();
            }
            else
            {
                lineRenderer.enabled = false;
            }
            
        }
    }

    IEnumerator ThrowPath()
    {
        while (isHolding)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane =  new Plane(Vector3.up,Vector3.zero);
            float disToGround = 0;
            path = new Vector3[resolution];
            if (plane.Raycast(ray,out disToGround))
            {
                endPos = ray.GetPoint(disToGround);
                for (int i = 0;i < resolution;i++)
                {
                    float percent = (i + 1)/(float)resolution;
                    path[i] = Utilities.GetBezierPoint(percent,starPos.position,endPos,height);
                }
                lineRenderer.positionCount = resolution;
                lineRenderer.SetPositions(path);
                lineRenderer.enabled = true;
                
                // #region 
                // Vector3 center = (transform.position + endPos) * 0.5f + Vector3.up * height;
                // LeanTween.drawBezierPath(transform.position,center,center,endPos);
                // #endregion
            }
            yield return null;
        }
    }

    public void Throw()
    {
        GameObject grenade = ObjectPooler.instance.SpawnFromPool("Grenade",starPos.position,Quaternion.identity);
        grenade.transform.DOPath(path, 0.8f, PathType.CatmullRom).SetEase(Ease.Linear);
        lineRenderer.enabled = false;
        grenadeAmount--;
    }

}
