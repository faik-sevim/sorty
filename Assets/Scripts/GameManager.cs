using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public TMP_Text levelValue;
    public TMP_Text Move;
    public GameObject cellPrefab;
    public GameObject MoveDisplayObject;
    public GameObject levelDisplayObject;
    public Transform rowPrefab;
    public Level[] levels;
    private Level currentLevelData;
    private int currentLevel = 1;
    private int currentMove = 1;
    public float animationSpeed = 0.5f;
    public Transform boardTransform;
    public CanvasGroup gameover;
    public CanvasGroup canvasGroup;
    public List<Transform> rowList = new List<Transform>();
    private EventSystem eventSystem;
    void Start()
    {
        LoadLevel(currentLevel);
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false; // Bu satýrý ekleyin
        eventSystem = EventSystem.current;

    }
    private Transform CreateNewRow()
    {
        Transform newRow = Instantiate(rowPrefab, boardTransform);
        rowList.Add(newRow);
        return newRow;
    }
    void SetupCells()
    {
        // Mevcut hücreleri temizleyin
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
            if (totalCells % 2 == 0) // Çift ise
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
            cellsInFirstRow = totalCells; // 5'ten küçükse tüm hücreleri ilk sýraya koy
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
            LoadLevel(currentLevelData.levelNumber + 1);
            eventSystem.enabled = true;
        }
        else
        {
            GameOver();
        }
    }

    void LoadLevel(int levelNumber)
    {

        currentLevelData = levels[levelNumber - 1];
        currentMove = currentLevelData.moves;
        levelValue.text = currentLevelData.levelNumber.ToString();
        SetupCells();
        UpdateMoveText();
        StartCoroutine(AnimateLevelDisplayObject());
    }

    private void UpdateMoveText()
    {
        Move.text = currentMove.ToString();
        StartCoroutine(AnimateMoveDisplayObject());
    }
    public void RestartGame()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        LoadLevel(1);
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
    private IEnumerator AnimateMoveDisplayObject()
    {
        Vector3 originalScale = MoveDisplayObject.transform.localScale; // Orijinal boyutu saklayýn
        Vector3 expandedScale = originalScale * 1.2f; // Büyütülecek boyutu belirleyin

        // Büyütme animasyonu
        float currentTime = 0f;
        float duration = 0.2f; // Animasyon süresi 0.2 saniye
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            MoveDisplayObject.transform.localScale = Vector3.Lerp(originalScale, expandedScale, currentTime / duration);
            yield return null;
        }

        // Küçültme animasyonu
        currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            MoveDisplayObject.transform.localScale = Vector3.Lerp(expandedScale, originalScale, currentTime / duration);
            yield return null;
        }

        MoveDisplayObject.transform.localScale = originalScale; // Son olarak orijinal boyuta geri dönüþ yapýn
    }
    private IEnumerator AnimateLevelDisplayObject()
    {
        Vector3 originalScale = levelDisplayObject.transform.localScale; // Orijinal boyutu saklayýn
        Vector3 expandedScale = originalScale * 1.2f; // Büyütülecek boyutu belirleyin

        // Büyütme animasyonu
        float currentTime = 0f;
        float duration = 0.2f; // Animasyon süresi 0.2 saniye
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            levelDisplayObject.transform.localScale = Vector3.Lerp(originalScale, expandedScale, currentTime / duration);
            yield return null;
        }

        // Küçültme animasyonu
        currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            levelDisplayObject.transform.localScale = Vector3.Lerp(expandedScale, originalScale, currentTime / duration);
            yield return null;
        }

        levelDisplayObject.transform.localScale = originalScale; // Son olarak orijinal boyuta geri dönüþ yapýn
    }

}
