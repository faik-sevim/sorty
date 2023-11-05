using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // TextMeshPro i�in eklenmesi gereken namespace

public class Cell : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
   
    private CellMover cellmover;
    private Cell selectedCell;
    private int number; // H�crenin tuttu�u say�y� temsil eden �zellik
    public TMP_Text numberText; // H�crenin �zerindeki TextMeshPro bile�eni
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

    // H�crenin tuttu�u say�y� almak i�in kullan�l�r.
    public int GetNumber()
    {
        return number;
    }

    // H�crenin tuttu�u say�y� ayarlamak i�in kullan�l�r.
    public void SetNumber(int value)
    {
        number = value;
        if (numberText == null)
        {
            Debug.LogWarning("numberText is not assigned!");
            return;  // E�er null ise, fonksiyonun geri kalan�n� �al��t�rmadan ��k.
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
        // Bu fonksiyonda �imdilik bir �ey yapmayaca��z, ��nk� sadece kayd�rma bitiminde hareketi kontrol ediyoruz.
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