using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour,IPooledObject
{
    public Rigidbody rigidbody;
    Material mat;
    Color initialColor;
    [SerializeField]public float forceMin;
    [SerializeField]public float forceMax;
    [SerializeField]public float lifeTime;
    [SerializeField]public float fadeTime;
    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        mat = GetComponent<Renderer>().material;
        initialColor = mat.color;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnObjectSpawn()
    {
        float force = UnityEngine.Random.Range(forceMin,forceMax);
        rigidbody.AddForce(force * transform.right);
        rigidbody.AddTorque(Random.insideUnitSphere * force);
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifeTime);
        float percent = 0;
        float fadeSpeed = 1/fadeTime;
        while (percent < 1)
        {
            percent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(initialColor,Color.clear,percent);
            yield return null;
        }
        mat.color = initialColor;
        gameObject.SetActive(false);
    }

}
