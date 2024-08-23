using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
   public int tileNumber;
    private GridManager gridManager;
    private Vector3 startPosition;
    private Vector3 dragDirection;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>(); // Find and link the GridManager instance
    }

    // Sets the number of the tile and updates its visual representation
    public void SetNumber(int number)
    {
        tileNumber = number;
        UpdateTileText();

        if (tileNumber == 16)
        {
            GetComponentInChildren<TextMeshProUGUI>().text = ""; // Hide text for the empty tile
            GetComponent<Image>().enabled = false; // Hide the image for the empty tile
        }
        else
        {
            GetComponent<Image>().enabled = true; // Ensure the tile is visible
        }
    }

    // Updates the text on the tile based on its number
    private void UpdateTileText()
    {
        TextMeshProUGUI textMesh = GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh != null && tileNumber != 16)
        {
            textMesh.text = tileNumber.ToString();
        }
    }

    // Stores the initial position when the drag starts
    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
    }

    // Tracks the drag direction as the tile is being dragged
    public void OnDrag(PointerEventData eventData)
    {
        dragDirection = (Vector3)eventData.position - startPosition;
    }

    // When the drag ends, checks if the tile is being dragged towards the empty tile and moves it if true
    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsDraggingTowardsEmptyTile())
        {
            gridManager.TryMoveTiles(this);
        }
        else
        {
            transform.position = startPosition; // Reset position if the drag is invalid
        }
    }

    // Checks if the drag direction is towards the empty tile
    private bool IsDraggingTowardsEmptyTile()
    {
        Vector3Int emptyPosition = gridManager.GetTileGridPosition(gridManager.emptyTile);
        Vector3Int currentPosition = gridManager.GetTileGridPosition(this);

        if (emptyPosition.y == currentPosition.y)
        {
            if ((emptyPosition.x > currentPosition.x && dragDirection.x > Mathf.Abs(dragDirection.y)) ||
                (emptyPosition.x < currentPosition.x && dragDirection.x < -Mathf.Abs(dragDirection.y)))
            {
                return true;
            }
        }
        else if (emptyPosition.x == currentPosition.x)
        {
            if ((emptyPosition.y > currentPosition.y && dragDirection.y < -Mathf.Abs(dragDirection.x)) ||
                (emptyPosition.y < currentPosition.y && dragDirection.y > Mathf.Abs(dragDirection.x)))
            {
                return true;
            }
        }

        return false;
    }
}