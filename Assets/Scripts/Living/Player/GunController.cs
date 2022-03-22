using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public enum FireMode
    {
        Auto,
        Brust,
        Single,
        ShotGun
    }
    [Header("Gun")]
    public Gun equipedGun;
    public Gun[] allGuns;
    public GameObject weaponHolder;
    public event Action<float> onGunChanged;//修改子弹伤害
    public event Action onEquipGun;//修改模型和删除地上的装备的模型
    public event Action<int,int> onShoot;//1: bulletPerMagzine , 2: remainingBulletInMag,用于UI显示
    public event Action<float> onReload;//换弹效果

    // Start is called before the first frame update
    void Start()
    {
        if (equipedGun == null)
        {
            EquipGun(allGuns[1]);//装备0,pistol;1,MachineGun;2,Famas;3,ShotGun
        }
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
    }

    private void LateUpdate() {
        if (equipedGun != null)
        {
            if (equipedGun.CheckToReload())
            {
                Reload();
            }
        }
    }

    private void Shoot()
    {
        if (equipedGun != null)
        {
            if (Input.GetMouseButton(0))//hold left button
            {
                equipedGun.onTriggerHold();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                equipedGun.onTriggerRelease();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (onEquipGun != null)
            {
                onEquipGun();
            }
        }

        //UI
        if (onShoot != null)
        {
            onShoot(equipedGun.bulletPerMagzine,equipedGun.remainingBulletInMag);
        }
    }

    public void EquipGun(Gun goToEquip)
    {
        if (equipedGun != null)
        {
            Destroy(equipedGun);
        }
        if (goToEquip != null)
        {
            equipedGun = Instantiate(goToEquip,weaponHolder.transform.position,weaponHolder.transform.rotation,weaponHolder.transform);
            equipedGun.TurnOffLight();
            equipedGun.EliminateGravity();
            if (onGunChanged != null)
            {
                onGunChanged(equipedGun.damage);
            }
        }
    }
    #region 
    public void EquipGun(GameObject goToEquip)//捡起枪
    {
        if (equipedGun != null)
        {
            // Destroy(equipedGun);
            Destroy(equipedGun.gameObject);
            // Destroy(weaponHolder.transform.GetChild(0).gameObject);
        }
        if (goToEquip != null)
        {
            equipedGun = goToEquip.GetComponent<Gun>();
            if (equipedGun != null)
            {
                goToEquip.transform.position = weaponHolder.transform.position;
                goToEquip.transform.rotation = weaponHolder.transform.rotation;
                goToEquip.transform.SetParent(weaponHolder.transform);
                goToEquip.transform.localScale = new Vector3(1,1,1);
                equipedGun.TurnOffLight();
                equipedGun.EliminateGravity();
            }
            if (onGunChanged != null && equipedGun != null)
            {
                onGunChanged(equipedGun.damage);
            }
        }
    }
    #endregion

    public void Reload()
    {
        if (equipedGun != null)
        {
            equipedGun.Reload();
            // if (onReload != null)
            // {
            //     onReload(equipedGun.reloadSpeed);
            // }
            onReload?.Invoke(equipedGun.reloadSpeed);
        }
    }
}
