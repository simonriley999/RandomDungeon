using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Health")]
    public Image healthPoint;
    public Image healthPointBG;
    [SerializeField]public float effectSpeed;
    public PlayerController playerController;
    public Boss boss;
    [Header("Ammo")]
    public Image ammoPoint;
    public GunController gunController;
    [Header("Game Done")]
    public RoomGenerator roomGenerator;//用于通关结算
    public GameObject playerCanvas;
    public GameObject gameOver;
    public GameObject victory;
    [Header("Game Pause")]
    public GameObject pauseCanvas;
    public static bool isPaused;
    // Start is called before the first frame update
    void Start()
    {
        if (playerController != null)
        {
            playerController.onHealthChange += HealthPointEffect;
            playerController.onDeath += ShowGameOverUI;
        }
        if (gunController != null)
        {
            gunController.onShoot += AmmoPointEffect;
            gunController.onReload += ReloadEffect;
        }
        // if (boss != null)
        // {
        //     boss.onDeath += ShowVictoryUI;
        // }
        if (roomGenerator != null)
        {
            roomGenerator.onVictory += ShowVictoryUI;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                
            }
            else
            {
                Pause();
            }
        }
    }

    private void HealthPointEffect(float _health,float _maxHealth)
    {
        healthPoint.fillAmount = _health / _maxHealth;
        StartCoroutine(HealthShadowEffect());
    }

    IEnumerator HealthShadowEffect()
    {
        while (healthPointBG.fillAmount > healthPoint.fillAmount)
        {
            healthPointBG.fillAmount -= Time.deltaTime * effectSpeed;
            yield return null;
        }
    }

    private void AmmoPointEffect(int _bulletPerMag,int _remainingBullet)
    {
        ammoPoint.fillAmount = (float)_remainingBullet / (float)_bulletPerMag;
    }

    private void ReloadEffect(float _speed)
    {
        Debug.Log("Reload");
        StartCoroutine(ReloadAnimation(_speed));
    }

    IEnumerator ReloadAnimation(float _speed)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * _speed;
            if (percent > 1)
            {
                percent = 1;
            }
            ammoPoint.fillAmount = percent;
            yield return null;
        }
    }

    void ShowGameOverUI()
    {
        playerCanvas.SetActive(false);
        gameOver.SetActive(true);
        if (playerController != null)
        {
            playerController.onHealthChange -= HealthPointEffect;
            playerController.onDeath -= ShowGameOverUI;
        }
        if (gunController != null)
        {
            gunController.onShoot -= AmmoPointEffect;
            gunController.onReload -= ReloadEffect;
        }
    }

    void ShowVictoryUI()
    {
        playerCanvas.SetActive(false);
        victory.SetActive(true);
        // if (boss != null)
        // {
        //     boss.onDeath -= ShowVictoryUI;
        // }
    }

    void Pause()
    {
        pauseCanvas.SetActive(true);
        playerCanvas.SetActive(false);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        pauseCanvas.SetActive(false);
        playerCanvas.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
    }

}
