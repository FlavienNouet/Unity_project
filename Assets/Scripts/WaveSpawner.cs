using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public SimpleObjectPool enemyPool;
    public Transform[] spawnPoints;

    public int startingEnemies = 5;
    public float spawnInterval = 0.4f;
    public float timeBetweenWaves = 5f;

    int currentWave = 0;

    void Start()
    {
        StartCoroutine(WaveLoop());
    }

    IEnumerator WaveLoop()
    {
        while (true)
        {
            currentWave++;
            int enemiesThisWave = 1 + (currentWave - 1) * 2;
            
            for (int i = 0; i < enemiesThisWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnInterval);
            }

            while (AreEnemiesAlive())
            {
                yield return null;
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    void SpawnEnemy()
{
    if (enemyPool == null || spawnPoints == null || spawnPoints.Length == 0) return;

    var go = enemyPool.GetFromPool();
    
    // SÉCURITÉ : Si le pool n'a plus de zombie disponible, on arrête pour éviter de faire crash la coroutine
    if (go == null) 
    {
        Debug.LogWarning("Plus de zombies disponibles dans le pool ! Augmente la taille de ton pool.");
        return; 
    }

    var spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
    go.transform.position = spawn.position;
    go.transform.rotation = spawn.rotation;
    go.SetActive(true);
}

    bool AreEnemiesAlive()
{
    // On parcourt TOUS les enfants du pool d'ennemis
    foreach (Transform child in enemyPool.transform)
    {
        // activeInHierarchy vérifie si l'objet ET ses parents sont actifs
        if (child.gameObject.activeInHierarchy)
        {
            return true; // Il reste au moins un zombie en vie, on attend
        }
    }
    return false; // Plus aucun zombie actif, on passe à la vague suivante !
}
}