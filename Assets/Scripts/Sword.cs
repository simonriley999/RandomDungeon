using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Sword : MonoBehaviour
{
    public event Action onMeleeAttackOver;
    bool isAttacking;
    public GameObject swordBody;
    [SerializeField]public float swingSpeed;
    // Start is called before the first frame update
    void Start()
    {
        isAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!LeanTween.isTweening(gameObject) && isAttacking)//挥剑结束
        {
            swordBody.SetActive(false);
            swordBody.transform.rotation = Quaternion.Euler(0,90f,0);
            isAttacking = false;
            onMeleeAttackOver?.Invoke();
        }
    }

    public void SwingSword()
    {
        swordBody.SetActive(true);
        LeanTween.rotateY(gameObject ,270f ,1f / swingSpeed);
        isAttacking = true;
    }
}
