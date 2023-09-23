using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Animator canvasAnimator;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Button[] colorButtons;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GoBackToMenu()
    {
        canvasAnimator.SetTrigger("showMenu");
    }

    public void ShowCustomizationScreen()
    {
        canvasAnimator.SetTrigger("showShop");
        var currentMoney = PlayerPrefs.GetInt("Money", 0);
        moneyText.text = currentMoney.ToString();
        if (currentMoney <= 300)
        {
            foreach (var button in colorButtons)
            {
                button.interactable = false;
            }
        }
        else
        {
            for (int i = 0; i < colorButtons.Length; i++)
            {
                if (PlayerPrefs.GetInt("Color", 0) == i)
                {
                    colorButtons[i].interactable = false;
                }
            }
        }
    }

    public void BuyColor(int index)
    {
        var currentMoney = PlayerPrefs.GetInt("Money", 0);
        if (currentMoney >= 300)
        {
            currentMoney -= 300;
            PlayerPrefs.SetInt("Money", currentMoney);
            moneyText.text = currentMoney.ToString();
            PlayerPrefs.SetInt("Color", index);
        }
        if (currentMoney <= 300)
        {
            foreach (var button in colorButtons)
            {
                button.interactable = false;
            }
        }
        for (int i = 0; i < colorButtons.Length; i++)
        {
            if (PlayerPrefs.GetInt("Color", 0) == i)
            {
                colorButtons[i].interactable = false;
            }
            else
            {
                colorButtons[i].interactable = true;
            }
        }
    }
}
