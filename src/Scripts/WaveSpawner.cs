using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct Wave
    {
        public Enemy[] enmies;
        public int count;
        public float timeBtwSpawn;
    }

    [SerializeField] Wave[] waves;

    [SerializeField] Transform[] spawnPoints;
    [SerializeField] float timeBtwWaves;

    Wave currentwWave;
    [HideInInspector] public int currentwWaveIndex;
    Transform player;

    bool isSpawnFinished = false;

    [SerializeField] TextMeshProUGUI waveText;

    bool isFreeTime = true;
    float curtimeBtwWaves;

    [SerializeField] GameObject spawnEffect;

    [SerializeField] AudioClip waveCompleteClip;

    private void Start()
    {
        player = Player.instance.transform;

        curtimeBtwWaves = timeBtwWaves;

        UpdateTExt();

        StartCoroutine(CallNextWave(currentwWaveIndex));
    }

    private void Update()
    {
        UpdateTExt();

        if (isSpawnFinished && GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            isSpawnFinished = false;

            if(currentwWaveIndex + 1 < waves.Length)
            {
                currentwWaveIndex++;
                StartCoroutine(CallNextWave(currentwWaveIndex));
            }
            else
            {
                //Создание босса
            }
        }
    }

    void UpdateTExt()
    {
        if (isFreeTime) waveText.text = "До следующей волны: " + ((int)(curtimeBtwWaves -= Time.deltaTime)).ToString();
        else waveText.text = "Текущая волна: " + currentwWaveIndex.ToString();
    }

    IEnumerator CallNextWave(int waveIndex)
    {
        curtimeBtwWaves = timeBtwWaves;

        isFreeTime = true;
        SoundManager.instance.PlayerSound(waveCompleteClip);

        yield return new WaitForSeconds(timeBtwWaves);
        isFreeTime = false;
        StartCoroutine(SpawWave(waveIndex));
    }

    IEnumerator SpawWave(int waveIndex)
    {
        currentwWave = waves[waveIndex];

        for (int i = 0; i < currentwWave.count; i++)
        {
            if (player == null) yield break;

            Enemy randomEnemy = currentwWave.enmies[Random.Range(0, currentwWave.enmies.Length)];
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            Instantiate(randomEnemy, randomSpawnPoint.position, Quaternion.identity);
            Instantiate(spawnEffect, randomSpawnPoint.position, Quaternion.identity);

            if(i == currentwWave.count - 1)
            {
                isSpawnFinished = true;
            }
            else
            {
                isSpawnFinished = false;
            }

            yield return new WaitForSeconds(currentwWave.timeBtwSpawn);
        }
    }
}
