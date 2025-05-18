using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    [SerializeField] Button[] buyButtons;
    [SerializeField] TextMeshProUGUI[] boughtTexts;
    [SerializeField] int[] prices;

    [SerializeField] GameObject shopPanel;

    public delegate void BuySeconPosition();
    public event BuySeconPosition buySeconPosition;

    public static Shop instance;

    [SerializeField] AudioClip popSound, succesBuyClip;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < buyButtons.Length; i++)
        {
            if (!PlayerPrefs.HasKey("Position" + i))
            {
                PlayerPrefs.SetInt("Position" + i, 0);
            }
            else
            {
                if (PlayerPrefs.GetInt("Position" + i) == 1)
                {
                    buyButtons[i].interactable = false;
                    boughtTexts[i].text = "Купленно";

                    if (i == 2) buySeconPosition.Invoke();
                }
            }
        }

        Check();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            shopPanel.SetActive(!shopPanel.activeInHierarchy);
            Check();

            SoundManager.instance.PlayerSound(popSound);

            if (shopPanel.activeInHierarchy) Time.timeScale = 0;
            else Time.timeScale = 1; Cursor.visible = false;
        }
    }

    void Check()
    {
        for (int i = 0; i < buyButtons.Length; i++)
        {
            if (Player.instance.currentMoney < prices[i])
            {
                buyButtons[i].interactable = false;
                boughtTexts[i].text = "Мало монет";
            }
            else
            {
                buyButtons[i].interactable = true;
                boughtTexts[i].text = "Купить";
            }

            if (PlayerPrefs.GetInt("Position" + i) == 1)
            {
                buyButtons[i].interactable = false;
                boughtTexts[i].text = "Купленно";
            }
        }
    }

    public void Buy(int index)
    {
        buyButtons[index].interactable = false;
        boughtTexts[index].text = "Купленно";

        SoundManager.instance.PlayerSound(succesBuyClip);

        PlayerPrefs.SetInt("Position" + index, 1);

        if (index == 2) buySeconPosition.Invoke();

        Player.instance.AddMoney(-prices[index]);

        Check();
    }

    [ContextMenu("Delete Player Prefs")]
    void DeletePlayerPrefs() => PlayerPrefs.DeleteAll();
}
