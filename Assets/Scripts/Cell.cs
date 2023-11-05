using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // TextMeshPro için eklenmesi gereken namespace

public class Cell : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
   
    private CellMover cellmover;
    private Cell selectedCell;
    private int number; // Hücrenin tuttuðu sayýyý temsil eden özellik
    public TMP_Text numberText; // Hücrenin üzerindeki TextMeshPro bileþeni
    void Start()
    {
        cellmover = FindObjectOfType<CellMover>();
        if (cellmover == null)
        {
            Debug.LogError("CellMover component not found in the scene!");
        }
        if (numberText == null)
        {
            numberText = this.transform.Find("NumberText").GetComponent<TMP_Text>();
        }
        
    }

    // Hücrenin tuttuðu sayýyý almak için kullanýlýr.
    public int GetNumber()
    {
        return number;
    }

    // Hücrenin tuttuðu sayýyý ayarlamak için kullanýlýr.
    public void SetNumber(int value)
    {
        number = value;
        if (numberText == null)
        {
            Debug.LogWarning("numberText is not assigned!");
            return;  // Eðer null ise, fonksiyonun geri kalanýný çalýþtýrmadan çýk.
        }
        numberText.text = number.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectCell(this);
        Debug.Log("Cell selected.");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SelectCell(this);
    }
    public void SelectCell(Cell cell)
    {
        selectedCell = cell;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Bu fonksiyonda þimdilik bir þey yapmayacaðýz, çünkü sadece kaydýrma bitiminde hareketi kontrol ediyoruz.
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        Vector3 dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
        DraggedDirection direction = GetDragDirection(dragVectorDirection);
        switch (direction)
        {
            case DraggedDirection.Right:
                cellmover.MoveCell(this, "Right");
                Debug.Log("Cell dragged to the right.");
                break;
            case DraggedDirection.Left:
                cellmover.MoveCell(this, "Left");
                Debug.Log("Cell dragged to the left.");
                break;
            case DraggedDirection.Up:
                cellmover.MoveCell(this, "Up");
                Debug.Log("Cell dragged upwards.");
                break;
            case DraggedDirection.Down:
                cellmover.MoveCell(this, "Down");
                Debug.Log("Cell dragged downwards.");
                break;
        }
    }

    private enum DraggedDirection
    {
        Up,
        Down,
        Right,
        Left
    }

    private DraggedDirection GetDragDirection(Vector3 dragVector)
    {
        float positiveX = Mathf.Abs(dragVector.x);
        float positiveY = Mathf.Abs(dragVector.y);
        if (positiveX > positiveY)
        {
            return (dragVector.x > 0) ? DraggedDirection.Right : DraggedDirection.Left;
        }
        else
        {
            return (dragVector.y > 0) ? DraggedDirection.Up : DraggedDirection.Down;
        }
    }

}