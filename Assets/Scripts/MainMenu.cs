using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // UnityEngine.Rendering.DebugUI yerine bu namespace kullanýlmalý
using UnityEngine.SceneManagement;
using System;

public class MainMenu : MonoBehaviour
{
    public GameManager GameManager;
    public TMP_Text levelText;
    public TMP_Text livesText;
    public Button button;
    public TMP_Text timerText;
    // Start is called before the first frame update
    void Start()
    {
        ColorBlock colors = button.colors;
        button.enabled = false;
        if (!PlayerPrefs.HasKey("Lives")) // Eðer "Lives" için bir deðer yoksa
        {
            PlayerPrefs.SetInt("Lives", 3); // Can sayýsýný 3 olarak ayarla
            PlayerPrefs.SetInt("CurrentLevel", 1); // Seviyeyi 1 olarak ayarla
            PlayerPrefs.Save(); // Deðiþiklikleri kaydet
        }
        Debug.LogWarning("Mainmenustar");
        int currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        int lives = PlayerPrefs.GetInt("Lives", 3);
        levelText.text = "Level: " + currentLevel;
        livesText.text = "Lives: " + lives;
        if (lives <= 0)
        {
            colors.disabledColor = new Color(1, 1, 1, 0.5f); //
            button.enabled = false;
        }
        else
        {
            button.enabled = true;

        }

    }

    public void PlayGame()
    {
        // Burada "GameScene" oyun sahnenizin adý olmalýdý
        SceneManager.LoadScene("Sorty");
    }
    public void FreshLoader()
    {
        Debug.LogWarning("Freshloader");
        PlayerPrefs.SetInt("Lives", 3); // Can sayýsýný 3 olarak ayarlayýn
        PlayerPrefs.SetInt("CurrentLevel", 1); // Seviyeyi 1 olarak ayarlayýn
        PlayerPrefs.Save(); // Deðiþiklikleri kaydedin
        SceneManager.LoadScene("MainMenu");
    }
}
