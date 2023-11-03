using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CellMover : MonoBehaviour
{
    public float animationSpeed = 0.5f;
    private bool isAnimating = false;
    public GameManager gameManager;
    public void MoveCell(Cell cell, string direction)
    {
        List<Transform> rowList = gameManager.GetRowList();
        if (isAnimating) return;

        Transform currentRow = cell.transform.parent;
        int currentIndex = cell.transform.GetSiblingIndex();
        Vector3 targetPosition = Vector3.zero;
        Transform displacedCell = null;

        int currentRowIndex = rowList.IndexOf(currentRow);
        int totalRows = rowList.Count;

        switch (direction)
        {
            case "Right":
                targetPosition = rowList[0].GetChild(0).position;

                StartCoroutine(AnimateMove(cell.transform, rowList[0].GetChild(0), targetPosition, cell.transform.position, () =>
                {
                    if (currentRowIndex == 0)
                    {
                        // Kaydýrýlan kutucuk zaten 1. row'da. Direkt olarak ilk pozisyona koy.
                        cell.transform.SetParent(rowList[0]);
                        cell.transform.SetAsFirstSibling();
                    }
                    else if (currentRowIndex == 1)
                    {
                        // Kaydýrýlan kutucuk 2. row'da. 
                        // 1. row'un son kutusunu 2. row'un baþýna koy.
                        var lastCellOfFirstRow = rowList[0].GetChild(rowList[0].childCount - 1);
                        lastCellOfFirstRow.SetParent(rowList[1]);
                        lastCellOfFirstRow.SetAsFirstSibling();

                        // Þimdi kaydýrýlan kutucuðu 1. row'un baþýna koy.
                        cell.transform.SetParent(rowList[0]);
                        cell.transform.SetAsFirstSibling();
                    }
                    else if (currentRowIndex == 2)
                    {
                        // Kaydýrýlan kutucuk 3. row'da.
                        // 1. row'un son kutusunu 2. row'un baþýna koy.
                        var lastCellOfFirstRow = rowList[0].GetChild(rowList[0].childCount - 1);
                        lastCellOfFirstRow.SetParent(rowList[1]);
                        lastCellOfFirstRow.SetAsFirstSibling();

                        // 2. row'un son kutusunu 3. row'un baþýna koy.
                        var lastCellOfSecondRow = rowList[1].GetChild(rowList[1].childCount - 1);
                        lastCellOfSecondRow.SetParent(rowList[2]);
                        lastCellOfSecondRow.SetAsFirstSibling();

                        // Þimdi kaydýrýlan kutucuðu 1. row'un baþýna koy.
                        cell.transform.SetParent(rowList[0]);
                        cell.transform.SetAsFirstSibling();
                    }

                    gameManager.DecrementMoveCounter();
                }));
                break;

            case "Left":
                int lastRowIndex = rowList.Count - 1;
                targetPosition = rowList[lastRowIndex].GetChild(rowList[lastRowIndex].childCount - 1).position;

                StartCoroutine(AnimateMove(cell.transform, rowList[lastRowIndex].GetChild(rowList[lastRowIndex].childCount - 1), targetPosition, cell.transform.position, () =>
                {
                    if (lastRowIndex == 0)
                    {
                        // Kaydýrýlan kutucuk zaten 1. row'da. Direkt olarak son pozisyona koy.
                        cell.transform.SetParent(rowList[0]);
                        cell.transform.SetAsLastSibling();
                    }
                    else if (currentRowIndex == 0 && lastRowIndex == 1)
                    {
                        // Kaydýrýlan kutucuk 1. row'da ve 2 row var.
                        cell.transform.SetParent(rowList[1]);
                        cell.transform.SetAsLastSibling();

                        var firstCellOfSecondRow = rowList[1].GetChild(0);
                        firstCellOfSecondRow.SetParent(rowList[0]);
                        firstCellOfSecondRow.SetAsLastSibling();
                    }
                    else if (currentRowIndex == 0 && lastRowIndex == 2)
                    {
                        // Kaydýrýlan kutucuk 1. row'da ve 3 row var.
                        cell.transform.SetParent(rowList[2]);
                        cell.transform.SetAsLastSibling();

                        var firstCellOfThirdRow = rowList[2].GetChild(0);
                        firstCellOfThirdRow.SetParent(rowList[1]);
                        firstCellOfThirdRow.SetAsLastSibling();

                        var firstCellOfSecondRow = rowList[1].GetChild(0);
                        firstCellOfSecondRow.SetParent(rowList[0]);
                        firstCellOfSecondRow.SetAsLastSibling();
                    }
                    else if (currentRowIndex == 1 && lastRowIndex == 2)
                    {
                        // Kaydýrýlan kutucuk 2. row'da ve 3 row var.
                        cell.transform.SetParent(rowList[2]);
                        cell.transform.SetAsLastSibling();

                        var firstCellOfThirdRow = rowList[2].GetChild(0);
                        firstCellOfThirdRow.SetParent(rowList[1]);
                        firstCellOfThirdRow.SetAsLastSibling();
                    }
                    else if (currentRowIndex == 2)
                    {
                        // Kaydýrýlan kutucuk 3. row'da.
                        cell.transform.SetParent(rowList[2]);
                        cell.transform.SetAsLastSibling();
                    }

                    gameManager.DecrementMoveCounter();
                }));
                break;

            // "Up" hareketi için durum kontrolü
            case "Up":
                // Eðer seçili kutucuk (cell) þuanki sýrasýnda en üstte deðilse
                if (currentIndex > 0)
                {
                    targetPosition = currentRow.GetChild(currentIndex - 1).position;
                    displacedCell = currentRow.GetChild(currentIndex - 1);
                }
                // Eðer seçili kutucuk (cell) 2. satýrýn en üstünde ise
                else if (currentIndex == 0 && currentRowIndex == 1)
                {
                    // Hedef pozisyonunu bir önceki satýrýn en altýndaki kutucuðun pozisyonu olarak ayarla
                    targetPosition = rowList[currentRowIndex - 1].GetChild(rowList[currentRowIndex - 1].childCount - 1).position;
                    // Yer deðiþtirecek olan kutucuðu belirle
                    displacedCell = rowList[currentRowIndex - 1].GetChild(rowList[currentRowIndex - 1].childCount - 1);
                }
                // Eðer seçili kutucuk (cell) 3. satýrýn en üstünde ise
                else if (currentIndex == 0 && currentRowIndex == 2)
                {
                    // Hedef pozisyonunu bir önceki satýrýn en altýndaki kutucuðun pozisyonu olarak ayarla
                    targetPosition = rowList[currentRowIndex - 1].GetChild(rowList[currentRowIndex - 1].childCount - 1).position;
                    // Yer deðiþtirecek olan kutucuðu belirle
                    displacedCell = rowList[currentRowIndex - 1].GetChild(rowList[currentRowIndex - 1].childCount - 1);
                }
                // Eðer yukarýdaki koþullardan hiçbiri saðlanmýyorsa iþlemi sonlandýr
                else
                {
                    return;
                }

                // Animasyonu baþlat (cell'in hareketi için)
                StartCoroutine(AnimateMove(cell.transform, displacedCell, targetPosition, cell.transform.position, () =>
                {
                    if (currentIndex == 0)
                    {
                        Transform displacedCellCurrentRow = displacedCell.parent;

                        // Seçili kutucuðu (cell) yer deðiþtirecek olan kutucuðun satýrýna taþý ve en alta konumlandýr
                        cell.transform.SetParent(displacedCellCurrentRow);
                        cell.transform.SetAsLastSibling();

                        displacedCell.SetParent(currentRow);
                        displacedCell.SetAsFirstSibling();
                    }
                    else
                    {
                        cell.transform.SetSiblingIndex(currentIndex - 1);
                    }

                    gameManager.DecrementMoveCounter();
                }));
                break;


            case "Down":
                if (currentIndex < currentRow.childCount - 1)
                {
                    targetPosition = currentRow.GetChild(currentIndex + 1).position;
                    displacedCell = currentRow.GetChild(currentIndex + 1);
                }
                else if (currentIndex == currentRow.childCount - 1 && currentRowIndex == 0 && rowList.Count > 1)
                {
                    targetPosition = rowList[1].GetChild(0).position;
                    displacedCell = rowList[1].GetChild(0);
                }
                else if (currentIndex == currentRow.childCount - 1 && currentRowIndex == 1 && rowList.Count > 2)
                {
                    targetPosition = rowList[2].GetChild(0).position;
                    displacedCell = rowList[2].GetChild(0);
                }
                else
                {
                    return;
                }

                StartCoroutine(AnimateMove(cell.transform, displacedCell, targetPosition, cell.transform.position, () =>
                {
                    if (currentIndex == currentRow.childCount - 1)
                    {
                        Transform displacedCellCurrentRow = displacedCell.parent;

                        displacedCell.SetParent(currentRow);
                        displacedCell.SetAsLastSibling();

                        cell.transform.SetParent(displacedCellCurrentRow);
                        cell.transform.SetAsFirstSibling();
                    }
                    else
                    {
                        cell.transform.SetSiblingIndex(currentIndex + 1);
                    }

                    gameManager.DecrementMoveCounter();
                }));
                break;
        }
    }
    private IEnumerator AnimateMove(Transform movingCell, Transform displacedCell, Vector3 movingTarget, Vector3 displacedTarget, System.Action onComplete)
    {
        isAnimating = true;

        Vector3 movingInitial = movingCell.position;
        Vector3 displacedInitial = displacedCell.position;

        float journeyLength = animationSpeed;
        float startTime = Time.time;

        float fractionOfJourney = 0;

        while (fractionOfJourney < 1)
        {
            float distanceCovered = (Time.time - startTime) / animationSpeed;
            fractionOfJourney = distanceCovered / journeyLength;

            movingCell.position = Vector3.Lerp(movingInitial, movingTarget, fractionOfJourney);
            displacedCell.position = Vector3.Lerp(displacedInitial, displacedTarget, fractionOfJourney);

            yield return null;
        }

        movingCell.position = movingTarget;
        displacedCell.position = displacedTarget;

        isAnimating = false;
        onComplete?.Invoke();
    }
}
