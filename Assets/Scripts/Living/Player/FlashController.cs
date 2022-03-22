using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashController : MonoBehaviour
{
    public GameObject flashHolder;
    [SerializeField]public float falshTime;
    public Sprite[] flashSprites;
    public SpriteRenderer[] spriteRenderers;
    // Start is called before the first frame update
    void Start()
    {
        DeActivate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DeActivate()
    {
        flashHolder.SetActive(false);
    }

    public void Activate()
    {
        flashHolder.SetActive(true);
        int flashSpriteIndex = Random.Range(0,flashSprites.Length);
        for (int i=0;i<spriteRenderers.Length;i++)
        {
            spriteRenderers[i].sprite = flashSprites[flashSpriteIndex];
        }
        Invoke("DeActivate",falshTime);
    }
}
