using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public TMP_Text levelValue;
    public TMP_Text Move;
    public TMP_Text Health;
    public GameObject cellPrefab;
    public GameObject MoveDisplayObject;
    public GameObject levelDisplayObject;
    public Transform rowPrefab;
    public Level[] levels;
    private Level currentLevelData;
    public int currentLevel = 1;
    private int currentMove = 1;
    public float animationSpeed = 0.5f;
    public Transform boardTransform;
    public CanvasGroup livesover;
    private int lives = 3; // Kullan�c�n�n can say�s�
    public CanvasGroup canvasGroup;
    public List<Transform> rowList = new List<Transform>();
    private EventSystem eventSystem;
    private float timeToNextLife = 60f; // Bir sonraki can i�in bekleme s�resi (saniye)
    private float lifeTimer; // Can�n yenilenece�i s�reye kadar ge�en s�re


    void Start()
    {
        lifeTimer = timeToNextLife; // Zamanlay�c�y� s�f�rlay�n
        LoadSavedLevel();
        //LoadLevel(currentLevel);
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false; // Bu sat�r� ekleyin
        livesover.blocksRaycasts = false;
        livesover.alpha = 0;
        eventSystem = EventSystem.current;

    }


    private Transform CreateNewRow()
    {
        Transform newRow = Instantiate(rowPrefab, boardTransform);
        rowList.Add(newRow);
        return newRow;
    }
    void LoadSavedLevel()
    {
        Debug.LogWarning("Loading Saved Level");
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        lives = PlayerPrefs.GetInt("Lives", 3);
        Debug.LogWarning("Loaded Level: " + currentLevel + " Lives: " + lives);
        UpdateHealthText();
        LoadLevel(currentLevel);
    }
    public void SaveLevelAndLives()
    {
        Debug.LogWarning("Saving Level: " + currentLevel + " Lives: " + lives);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.SetInt("Lives", lives);
        PlayerPrefs.Save();
    }
    void SetupCells()
    {
        Debug.LogWarning("SetupCells");
        // Mevcut h�creleri temizleyin
        for (int i = rowList.Count - 1; i >= 0; i--)
        {
            foreach (Transform cell in rowList[i])
            {
                Destroy(cell.gameObject);
            }
            Destroy(rowList[i].gameObject);
        }
        rowList.Clear();

        int totalCells = currentLevelData.initialNumbers.Length;

        int cellsInFirstRow = 0;
        int cellsInSecondRow = 0;
        int cellsInThirdRow = 0;

        if (totalCells > 5 && totalCells <= 10)
        {
            if (totalCells % 2 == 0) // �ift ise
            {
                cellsInFirstRow = totalCells / 2;
                cellsInSecondRow = totalCells / 2;
            }
            else // Tek ise
            {
                cellsInFirstRow = (totalCells + 1) / 2;
                cellsInSecondRow = totalCells - cellsInFirstRow;
            }
        }
        else if (totalCells > 10)
        {
            if (totalCells % 3 == 0)
            {
                cellsInFirstRow = totalCells / 3;
                cellsInSecondRow = totalCells / 3;
                cellsInThirdRow = totalCells / 3;
            }
            else if (totalCells == 11)
            {
                cellsInFirstRow = 4;
                cellsInSecondRow = 4;
                cellsInThirdRow = 3;
            }
            else if (totalCells == 13)
            {
                cellsInFirstRow = 5;
                cellsInSecondRow = 4;
                cellsInThirdRow = 4;
            }
            else if (totalCells == 14)
            {
                cellsInFirstRow = 5;
                cellsInSecondRow = 5;
                cellsInThirdRow = 4;
            }
        }
        else
        {
            cellsInFirstRow = totalCells; // 5'ten k���kse t�m h�creleri ilk s�raya koy
        }

        int currentCellIndex = 0;

        Transform currentRow = CreateNewRow();
        for (int i = 0; i < cellsInFirstRow; i++)
        {
            GameObject newCell = Instantiate(cellPrefab, currentRow);
            Cell cellComponent = newCell.GetComponent<Cell>();
            cellComponent.SetNumber(currentLevelData.initialNumbers[currentCellIndex]);
            currentCellIndex++;
        }

        if (cellsInSecondRow > 0)
        {
            currentRow = CreateNewRow();
            for (int i = 0; i < cellsInSecondRow; i++)
            {
                GameObject newCell = Instantiate(cellPrefab, currentRow);
                Cell cellComponent = newCell.GetComponent<Cell>();
                cellComponent.SetNumber(currentLevelData.initialNumbers[currentCellIndex]);
                currentCellIndex++;
            }
        }

        if (cellsInThirdRow > 0)
        {
            currentRow = CreateNewRow();
            for (int i = 0; i < cellsInThirdRow; i++)
            {
                GameObject newCell = Instantiate(cellPrefab, currentRow);
                Cell cellComponent = newCell.GetComponent<Cell>();
                cellComponent.SetNumber(currentLevelData.initialNumbers[currentCellIndex]);
                currentCellIndex++;
            }
        }
    }

    public List<Transform> GetRowList()
    {
        return rowList;
    }
    public void DecrementMoveCounter()
    {
        Debug.LogWarning("DecrementMoveCounter");
        currentMove--;
        UpdateMoveText();
        if (currentMove == 0)
        {
            StartCoroutine(CheckGameResult());
            eventSystem.enabled = false;
        }
    }
    private IEnumerator CheckGameResult()
    {
        Debug.LogWarning("CheckGameResult");
        bool isCorrectOrder = true;

        int checkedCells = 0;

        foreach (Transform row in rowList)
        {
            for (int i = 0; i < row.childCount; i++)
            {
                Cell cell = row.GetChild(i).GetComponent<Cell>();
                //Debug.Log("Cell " + checkedCells + ": " + cell.GetNumber() + " - Expected: " + currentLevelData.correctOrder[checkedCells]);

                if (cell.GetNumber() != currentLevelData.correctOrder[checkedCells])
                {
                    isCorrectOrder = false;
                    break;
                }

                checkedCells++;

                if (checkedCells >= currentLevelData.correctOrder.Length)
                {
                    break;
                }
            }

            if (!isCorrectOrder || checkedCells >= currentLevelData.correctOrder.Length)
            {
                break;
            }
        }

        if (isCorrectOrder)
        {
            yield return new WaitForSeconds(1f);
            currentLevel = currentLevelData.levelNumber + 1; // G�ncel seviye numaras�n� art�r
            SaveLevelAndLives(); // Yeni seviye numaras�n� ve can say�s�n� kaydet
            LoadLevel(currentLevel); // Yeni seviyeyi y�kle
            eventSystem.enabled = true;
        }
        else
        {
            lives--; // Can say�s�n� azalt
            UpdateHealthText();
            SaveLevelAndLives();
            if (lives <= 0)
            {
                LivesOver();
            }
            else
            {
                UpdateHealthText();
                GameOver();
            }

        }
    }
    private void LoadLevel(int levelNumber)
    {
        Debug.LogWarning("LoadLevel");

        currentLevelData = levels[levelNumber - 1];
        currentMove = currentLevelData.moves;
        levelValue.text = currentLevelData.levelNumber.ToString();
        SetupCells();
        UpdateMoveText();
        StartCoroutine(AnimateLevelDisplayObject());
        UpdateHealthText();
    }
    private void UpdateMoveText()
    {
        Debug.LogWarning("UpdateMoveText");

        Move.text = currentMove.ToString();
        StartCoroutine(AnimateMoveDisplayObject());
    }
    private void UpdateHealthText()
    {
        Debug.LogWarning("UpdateHealthText");
        Health.text = lives.ToString();
    }
    public void RestartGame()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        LoadLevel(currentLevelData.levelNumber);
        Debug.LogWarning("restartgaame");

    }
    public void GameOver()
    {
        canvasGroup.blocksRaycasts = false; // Block UI interactions during fade in
        StartCoroutine(FadeIn());

        IEnumerator FadeIn()
        {
            float duration = 1.0f;
            float currentTime = 0;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                canvasGroup.alpha = currentTime / duration;
                yield return null;
            }

            canvasGroup.blocksRaycasts = true; // Allow UI interactions after fade in is complete
            eventSystem.enabled = true;
            canvasGroup.alpha = 1;
        }
    }
    public void LivesOver()
    {
        Debug.LogWarning("Livesover");

        livesover.blocksRaycasts = false; // Block UI interactions during fade in
        StartCoroutine(FadeIn());
        // Play tu�unu deaktif etme kodu
        PlayerPrefs.SetInt("Lives", 0); // Can say�s�n� 0 olarak kaydet
    

        IEnumerator FadeIn()
        {
            float duration = 1.0f;
            float currentTime = 0;

            while (currentTime < duration)
            {
                currentTime += Time.deltaTime;
                livesover.alpha = currentTime / duration;
                yield return null;
            }

            livesover.blocksRaycasts = true; // Allow UI interactions after fade in is complete
            eventSystem.enabled = true;
            livesover.alpha = 1;
        }
    }
    private IEnumerator AnimateMoveDisplayObject()
    {
        Vector3 originalScale = MoveDisplayObject.transform.localScale; // Orijinal boyutu saklay�n
        Vector3 expandedScale = originalScale * 1.2f; // B�y�t�lecek boyutu belirleyin

        // B�y�tme animasyonu
        float currentTime = 0f;
        float duration = 0.2f; // Animasyon s�resi 0.2 saniye
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            MoveDisplayObject.transform.localScale = Vector3.Lerp(originalScale, expandedScale, currentTime / duration);
            yield return null;
        }

        // K���ltme animasyonu
        currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            MoveDisplayObject.transform.localScale = Vector3.Lerp(expandedScale, originalScale, currentTime / duration);
            yield return null;
        }

        MoveDisplayObject.transform.localScale = originalScale; // Son olarak orijinal boyuta geri d�n�� yap�n
    }
    private IEnumerator AnimateLevelDisplayObject()
    {
        Vector3 originalScale = levelDisplayObject.transform.localScale; // Orijinal boyutu saklay�n
        Vector3 expandedScale = originalScale * 1.2f; // B�y�t�lecek boyutu belirleyin

        // B�y�tme animasyonu
        float currentTime = 0f;
        float duration = 0.2f; // Animasyon s�resi 0.2 saniye
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            levelDisplayObject.transform.localScale = Vector3.Lerp(originalScale, expandedScale, currentTime / duration);
            yield return null;
        }

        // K���ltme animasyonu
        currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            levelDisplayObject.transform.localScale = Vector3.Lerp(expandedScale, originalScale, currentTime / duration);
            yield return null;
        }

        levelDisplayObject.transform.localScale = originalScale; // Son olarak orijinal boyuta geri d�n�� yap�n
    }
    public void MainMenuLoader()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene("MainMenu");
        Debug.LogWarning("Mainmenuloader");
    }
    public void FreshLoader()
    {
        Debug.LogWarning("Freshloader");
        PlayerPrefs.SetInt("Lives", 3); // Can say�s�n� 3 olarak ayarlay�n
        PlayerPrefs.SetInt("CurrentLevel", 1); // Seviyeyi 1 olarak ayarlay�n
        PlayerPrefs.Save(); // De�i�iklikleri kaydedin
        SceneManager.LoadScene("MainMenu");
    }
    internal static void LoadLevel(object currentLevel)
    {
        throw new NotImplementedException();
       
    }
}
