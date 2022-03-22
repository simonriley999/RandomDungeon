using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour,IPooledObject
{
    public enum FireMode
    {
        Auto,
        Brust,
        Single,
        ShootGun
    }
    [Header("FireMode")]
    public FireMode fireMode;
    [SerializeField]public float fireRate;
    [SerializeField]public int brustCount;
    [SerializeField]public float damage;
    private float nextFireTime;
    public GameObject bulletPrefab;
    public Transform[] firePoint;
    public Transform shellPoint;
    public FlashController flashController;
    public bool triggerReleasedSinceLastShoot;
    int remainingInBrust;

    [Header("Magzine")]
    [SerializeField]public int bulletPerMagzine;
    public float reloadSpeed;
    public int remainingBulletInMag;
    bool isReloading;
    [Header("Pick")]
    public Rigidbody rigidbody;
    public Collider pickArea;
    public GameObject hintLight;
    public bool isCouldPick;
    public bool isCouldRotate;
    public GameObject pickUIHolder;
    public RectTransform pickUIRect;
    private GunController playerGun;
    [SerializeField]public float rotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
        nextFireTime = Time.time;
        remainingInBrust = brustCount;
        remainingBulletInMag = bulletPerMagzine;
        isReloading = false;
        isCouldPick = false;
        isCouldRotate = false;
        triggerReleasedSinceLastShoot = true;

        playerGun = GameObject.FindGameObjectWithTag("Player").GetComponent<GunController>();
    }

    private void Update() {
        Roatate();
        ShowPickUI();
    }


    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
        {
            Debug.Log("PlayerEnter");
            if (playerGun != null)
            {
                playerGun.onEquipGun += DestroyThisAndEquip;
            }
            isCouldPick = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player")
        {
            Debug.Log("PlayerExit");
            if (playerGun != null)
            {
                playerGun.onEquipGun -= DestroyThisAndEquip;
            }
            isCouldPick = false;
        }
    }

    private void Shoot()
    {
        if (Time.time>nextFireTime && remainingBulletInMag > 0 && !isReloading)
        {
            if (fireMode == FireMode.Brust)
            {
                if (remainingInBrust <= 0) 
                {
                    return;
                }
                remainingInBrust--;

            }
            else if (fireMode == FireMode.Single || fireMode == FireMode.ShootGun)
            {
                if (!triggerReleasedSinceLastShoot)
                {
                    return;
                }
            }
            nextFireTime = fireRate + Time.time;
            if (fireMode == FireMode.ShootGun)
            {
                for (int i=0;i<firePoint.Length;i++)
                {
                    ObjectPooler.instance.SpawnFromPool("Bullet",firePoint[i].position,firePoint[i].rotation);
                }
            }
            else
            {
                ObjectPooler.instance.SpawnFromPool("Bullet",firePoint[0].position,firePoint[0].rotation);
            }
            remainingBulletInMag--;
            // Instantiate(bulletPrefab,firePoint.position,firePoint.rotation);
            ObjectPooler.instance.SpawnFromPool("Shell",shellPoint.position,shellPoint.rotation);
            flashController.Activate();
            Debug.Log("fire");
        }
    }

    public void onTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShoot = false;
    }

    public void onTriggerRelease()
    {
        remainingInBrust = brustCount;
        triggerReleasedSinceLastShoot = true;
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            //利用UI做出统计换弹时间的动画//在UIManager中
            yield return null;
        }
        remainingBulletInMag = bulletPerMagzine;
        isReloading = false;
        
    }

    public void Reload()
    {
        if (!isReloading && remainingBulletInMag < bulletPerMagzine)
        {
            StartCoroutine(AnimateReload());
        }
    }

    public void OnObjectSpawn()
    {
        nextFireTime = Time.time;
        remainingInBrust = brustCount;
        remainingBulletInMag = bulletPerMagzine;
        isReloading = false;
    }

    public void TurnOffLight()
    {
        hintLight.SetActive(false);
    }

    private void ShowPickUI()
    {
        if (isCouldPick)
        {
            if (!pickUIHolder.activeSelf)
            {
                pickUIHolder.SetActive(true);
            }
            Utilities.UIFollowCharacter(pickUIRect,transform.position,Vector2.zero);
        }
        else
        {
            if (pickUIHolder.activeSelf)
            {
                pickUIHolder.SetActive(false);
            }
        }
    }

    public void EliminateGravity()
    {
        if (rigidbody != null)
        {
            rigidbody.useGravity = false;
            rigidbody.isKinematic = false;
        }
    }

    void DisabelPickArea()
    {
        pickArea.gameObject.SetActive(false);
    }

    private void DestroyThisAndEquip()
    {
        isCouldPick = false;
        isCouldRotate = false;
        playerGun.EquipGun(gameObject);
        // Destroy(gameObject);
        ShowPickUI();
        DisabelPickArea();
        TurnOffLight();
    }

    public bool CheckToReload() {
        if (!isReloading && remainingBulletInMag <= 0)
        {
            return true;
        }
        else{
            return false;
        }
    }

    public void SetCouldRoate()
    {
        isCouldRotate = true;
    }

    private void Roatate()
    {
        if (isCouldRotate)
        {
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime,Space.World);
        }
    }
}
