using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeathPanel : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI scoreText;

    [SerializeField] AudioClip popSound;


    private void Start()
    {
        WaveSpawner wsP = FindObjectOfType<WaveSpawner>();

        scoreText.text = "Текущая волна: " + wsP.currentwWaveIndex.ToString();
    }

    public void Restart()
    {
        SoundManager.instance.PlayerSound(popSound);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
