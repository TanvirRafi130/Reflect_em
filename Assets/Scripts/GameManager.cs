using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

[System.Serializable]
public class Wave
{
    public int level = 0;
    public EnemyType enemyTypeToSpawn;
    public int maxEnemy;
    public int maxENemiesAtATime;
    public float bulletDamage;
    public float enemyMoveSpeed;
    public float enemyStopDist;
    public float shootInterval;
    public float enemyMaxHealth;
}

[System.Serializable]
public class EnemyPrefabs
{
    public EnemyType enemyType;
    public List<GameObject> prefabs;
}
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;
    public List<GameObject> bullets;
    public List<Color> colors;
    public float bulletShootSpeed = 50f;
    public bool isPaused = false;

    [Header("Wave")]
    public float waveInterval;
    public List<Wave> waves;

    List<GameObject> enemies;
    public Transform spawnPositionsParent;

    [Header("Particles")]
    public ParticleSystem telePortParticle;
    public ParticleSystem playerCollidParticle;
    public ParticleSystem bulletHitParticle;

    public List<EnemyPrefabs> EnemyPrefabs;
    public Image waveBar;
    public TextMeshProUGUI waveText;

    int killLeft;

    int waveNumber = 0;
    int killsTotal = 0;
    public Wave currentWave;

    private void Awake()
    {
        if (instance == null) instance = this;
        enemies = new List<GameObject>();
    }

    private void Start()
    {
        StartCoroutine(WaveManager());
    }

    void SetWaveBar()
    {
        float e = (float)killsTotal / (float)currentWave.maxEnemy;
        waveBar.DOFillAmount(e, 0.3f);
        waveText.text = $"{currentWave.level + 1} / {waves.Count}";

    }

    IEnumerator WaveManager()
    {
        while (waveNumber < waves.Count)
        {
            currentWave = waves[waveNumber];
            killLeft = currentWave.maxEnemy;
            killsTotal = 0;
            SetWaveBar();

            // Wait for wave interval
            yield return new WaitForSeconds(waveInterval);

            // Spawn enemies for current wave
            while (killLeft > 0)
            {
                // Check if we can spawn more enemies
                if (enemies.Count < currentWave.maxENemiesAtATime)
                {
                    // Find a random spawn position
                    Transform spawnPos = spawnPositionsParent.GetChild(Random.Range(0, spawnPositionsParent.childCount));
                    var itemList = EnemyPrefabs.Where(x => x.enemyType.Equals(currentWave.enemyTypeToSpawn)).FirstOrDefault();
                    if (itemList != null && itemList.prefabs.Count > 0)
                    {
                        GameObject enemy = Instantiate(itemList.prefabs[Random.Range(0, itemList.prefabs.Count)], spawnPos.position, Quaternion.identity);
                        enemies.Add(enemy);

                        // Setup enemy properties based on wave
                        Enemy enemyScript = enemy.GetComponent<Enemy>();
                        if (enemyScript != null)
                        {
                            enemyScript.moveSpeed = currentWave.enemyMoveSpeed;
                            enemyScript.stopDistance = currentWave.enemyStopDist;
                            enemyScript.shootInterval = currentWave.shootInterval;
                            enemyScript.maxHealth = currentWave.enemyMaxHealth;
                        }

                        killLeft--;
                    }
                }

                // Wait before next spawn
                yield return new WaitForSeconds(1f);
            }

            // Wait until all enemies are dead
            while (enemies.Count > 0)
            {
                yield return new WaitForSeconds(1f);
            }

            waveNumber++;
        }

        // All waves completed
        Debug.Log("All waves completed!");
    }

    public void OnEnemyDeath(GameObject enemy)
    {
        killsTotal++;
        SetWaveBar();
        enemies.Remove(enemy);
    }
}
