using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // UnityEngine.Rendering.DebugUI yerine bu namespace kullan�lmal�
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
        if (!PlayerPrefs.HasKey("Lives")) // E�er "Lives" i�in bir de�er yoksa
        {
            PlayerPrefs.SetInt("Lives", 3); // Can say�s�n� 3 olarak ayarla
            PlayerPrefs.SetInt("CurrentLevel", 1); // Seviyeyi 1 olarak ayarla
            PlayerPrefs.Save(); // De�i�iklikleri kaydet
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
        // Burada "GameScene" oyun sahnenizin ad� olmal�d�
        SceneManager.LoadScene("Sorty");
    }
    public void FreshLoader()
    {
        Debug.LogWarning("Freshloader");
        PlayerPrefs.SetInt("Lives", 3); // Can say�s�n� 3 olarak ayarlay�n
        PlayerPrefs.SetInt("CurrentLevel", 1); // Seviyeyi 1 olarak ayarlay�n
        PlayerPrefs.Save(); // De�i�iklikleri kaydedin
        SceneManager.LoadScene("MainMenu");
    }
}
