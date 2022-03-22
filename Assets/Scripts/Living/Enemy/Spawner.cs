using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyHolder;
    public MapGenerator mapGenerator;
    public RoomGenerator roomGenerator;
    private Transform openTile;
    [Header("Enemy Generate")]
    [SerializeField]public int enemyCount;
    public int currentEnemyCount;
    private int currentRoomIndex;
    public GameObject enemyPrefab;
    public GameObject boss;
    [SerializeField]public float enemyRefreshRate;
    [SerializeField]public float spawnDelay;
    [SerializeField]public float tileFlashSpeed;
    bool isBoss;
    [Header("Difficulty")]
    [SerializeField]public float healthIncrement;
    [SerializeField]public float damageIncrement;
    [SerializeField]public float speedIncrement;
    [SerializeField]public int amountIncrement;
    int difficultyLevel;

    [Header("Props Generate")]
    public Gun[] allGuns;
    [Header("Sounds")]
    public SoundManager soundManager;

    private void Start() 
    {
        isBoss = false;
        currentEnemyCount = 0;
        difficultyLevel = 0;
        if (boss != null)
        {
            boss.GetComponent<Boss>().onDeath += EnemyDeath;
        }
        DoorEventSystem.instance.onSpawnEnemy += StartToEnemyGenerate;
    }
    // Start is called before the first frame update
    void StartToEnemyGenerate(int roomIndex)
    {
        currentRoomIndex = roomIndex;
        // DoorEventSystem.instance.onSpawnEnemy -= StartToEnemyGenerate;
        if (isBoss)
        {
            currentEnemyCount += 1;
            soundManager.Play("Boss");
            soundManager.SetVolume("Battle",0f);
            soundManager.SetVolume("Explore",0f);
            StartCoroutine(BossGenerate(roomIndex));
        }
        else
        {
            enemyCount += amountIncrement;
            currentEnemyCount += enemyCount;
            soundManager.SetVolume("Battle",1f);
            soundManager.SetVolume("Explore",0f);
            StartCoroutine(EnemyGenerate(roomIndex));
        }
    }

    IEnumerator BossGenerate(int roomIndex)
    {
        Transform rCenter = roomGenerator.GetRoomCenterTile(roomIndex);
        Material tileMat = openTile.GetComponent<Renderer>().material;
        Color originalColor = tileMat.color;
        Color flashColor = Color.red;
        float generateTimer = 0;
        while (generateTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(originalColor,flashColor,Mathf.PingPong(generateTimer * tileFlashSpeed,1));
            generateTimer += Time.deltaTime;
            yield return null;
        }
        tileMat.color = originalColor;
        boss.SetActive(true);
        Vector3 v = new Vector3(rCenter.position.x,0.5f,rCenter.position.z);
        boss.transform.position = v;
        yield return null;
    }

    IEnumerator EnemyGenerate(int roomIndex)
    {
        for (int i=0;i<enemyCount;i++)
        {
            openTile = roomGenerator.GetRandomOpenTile(roomIndex);
            Material tileMat = openTile.GetComponent<Renderer>().material;
            Color originalColor = tileMat.color;
            Color flashColor = Color.red;
            float generateTimer = 0;
            while (generateTimer < spawnDelay)
            {
                tileMat.color = Color.Lerp(originalColor,flashColor,Mathf.PingPong(generateTimer * tileFlashSpeed,1));
                generateTimer += Time.deltaTime;
                yield return null;
            }
            tileMat.color = originalColor;
            if (openTile.tag == "Obstacle")
            {
                Debug.LogError(openTile.position);
            }
            StartCoroutine(SpawnEnemy(openTile));
            yield return null;
        }
        
        // DoorEventSystem.instance.onSpawnEnemy += StartToEnemyGenerate;
    }

    IEnumerator SpawnEnemy(Transform _openTile)
    {
        GameObject enemy = Instantiate(enemyPrefab,_openTile.position,Quaternion.identity,enemyHolder.transform);
        enemy.GetComponent<EnemyController>().onDeath += EnemyDeath;
        EnemyController enemyProperty = enemy.GetComponent<EnemyController>();

        enemyProperty.maxHealth += healthIncrement * difficultyLevel;
        enemyProperty.damage += damageIncrement * difficultyLevel;
        enemyProperty.moveSpeed += speedIncrement * difficultyLevel;

        yield return new WaitForSeconds(enemyRefreshRate);
    }

    private void EnemyDeath()
    {
        currentEnemyCount--;
        if (currentEnemyCount <= 0)
        {
            roomGenerator.SetRoomClear(currentRoomIndex);
            roomGenerator.SetRoomUnlock(currentRoomIndex);
            Vector2Int rCenter = roomGenerator.GetRoomCenter(currentRoomIndex);
            Gun temp = Instantiate(allGuns[UnityEngine.Random.Range(0,allGuns.Length)],new Vector3(rCenter.x,0.5f,rCenter.y),Quaternion.Euler(90,90,0));
            temp.SetCouldRoate();
            difficultyLevel++;
            enemyCount += amountIncrement * difficultyLevel;
            // Instantiate(allGuns[UnityEngine.Random.Range(0,allGuns.Length)],roomGenerator.GetRandomOpenTile(currentRoomIndex));
            if (!isBoss)
            {
                soundManager.SetVolume("Battle",0f);
                soundManager.SetVolume("Explore",1f);
            }
            if (roomGenerator.IsTheLastRoom())
            {
                isBoss = true;
            }
            // DoorEventSystem.instance.onSpawnEnemy -= StartToEnemyGenerate;
        }
    }

    public void SpawnAssistance(int _amount)
    {
        for (int i=0;i<_amount;i++)
        {
            currentEnemyCount++;
            StartCoroutine(CSpawnAssitance());
        }
    }

    IEnumerator CSpawnAssitance()
    {
        Transform copenTile = roomGenerator.GetRandomOpenTile(currentRoomIndex);
        Material tileMat = copenTile.GetComponent<Renderer>().material;
        Color originalColor = tileMat.color;
        Color flashColor = Color.red;
        float generateTimer = 0;
        while (generateTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(originalColor,flashColor,Mathf.PingPong(generateTimer * tileFlashSpeed,1));
            generateTimer += Time.deltaTime;
            yield return null;
        }
        tileMat.color = originalColor;
        if (copenTile.tag == "Obstacle")
        {
            Debug.LogError(copenTile.position);
        }
        StartCoroutine(SpawnEnemy(copenTile));
    }
}
