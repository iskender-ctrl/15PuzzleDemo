using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public Transform numbers;
    public float tileSpacing = 100f;
    private List<Tile> tiles = new List<Tile>();
    public Tile emptyTile;
    private int gridSize = 4; // Grid size set to 4x4
    public GameObject donePanel;

    private void Start()
    {
        InitializeTiles();
        ShuffleTiles();
    }

    // Initializes tiles by finding all child objects of "numbers" and adding them to the tile list
    private void InitializeTiles()
    {
        tiles.Clear();
        foreach (Transform child in numbers)
        {
            Tile tile = child.GetComponent<Tile>();
            if (tile != null)
            {
                tiles.Add(tile);
            }
        }
    }

    // Shuffles the tiles and sets one as the empty tile (with number 16)
    private void ShuffleTiles()
    {
        List<int> numbersList = new List<int>();
        for (int i = 1; i <= 15; i++)
        {
            numbersList.Add(i);
        }

        int randomEmptyIndex = Random.Range(0, tiles.Count);
        for (int i = 0; i < tiles.Count; i++)
        {
            if (i == randomEmptyIndex)
            {
                tiles[i].SetNumber(16);
                emptyTile = tiles[i];
                emptyTile.GetComponent<Image>().enabled = false;
            }
            else
            {
                int randomIndex = Random.Range(0, numbersList.Count);
                tiles[i].SetNumber(numbersList[randomIndex]);
                tiles[i].GetComponent<Image>().enabled = true;
                numbersList.RemoveAt(randomIndex);
            }
        }

        if (emptyTile == null)
        {
            Debug.LogError("Empty tile not found! Ensure that one tile is assigned as the empty tile.");
        }
    }

    // Resets the level by reinitializing and shuffling the tiles
    public void NewsLevelButton()
    {
        InitializeTiles();
        ShuffleTiles();
    }

    // Attempts to move the selected tile if it is adjacent to the empty tile
    public void TryMoveTiles(Tile selectedTile)
    {
        Vector3Int emptyPosition = GetTileGridPosition(emptyTile);
        Vector3Int selectedPosition = GetTileGridPosition(selectedTile);

        if (emptyPosition.y == selectedPosition.y)
        {
            int direction = emptyPosition.x > selectedPosition.x ? 1 : -1;

            if ((direction == 1 && selectedPosition.x < emptyPosition.x) ||
                (direction == -1 && selectedPosition.x > emptyPosition.x))
            {
                StartCoroutine(MoveTilesSmoothly(selectedPosition.x, emptyPosition.x, direction, selectedPosition.y, true));
            }
        }
        else if (emptyPosition.x == selectedPosition.x)
        {
            int direction = emptyPosition.y > selectedPosition.y ? 1 : -1;

            if ((direction == 1 && selectedPosition.y < emptyPosition.y) ||
                (direction == -1 && selectedPosition.y > emptyPosition.y))
            {
                StartCoroutine(MoveTilesSmoothly(selectedPosition.y, emptyPosition.y, direction, selectedPosition.x, false));
            }
        }

        CheckLevelCompletion();
    }

    // Helper method to get the initial position of a tile based on its index
    private Vector3 GetTileInitialPosition(int index)
    {
        int row = index / gridSize;
        int col = index % gridSize;
        return new Vector3(col * tileSpacing, -row * tileSpacing, 0f);
    }

    // Smoothly moves tiles from one position to another
    private IEnumerator MoveTilesSmoothly(int start, int end, int direction, int fixedCoordinate, bool isHorizontal)
    {
        for (int i = end; i != start; i -= direction)
        {
            Tile tile;
            if (isHorizontal)
            {
                tile = GetTileAtPosition(i - direction, fixedCoordinate);
            }
            else
            {
                tile = GetTileAtPosition(fixedCoordinate, i - direction);
            }

            yield return StartCoroutine(SmoothSwap(tile, emptyTile));
        }
    }

    // Smoothly swaps the positions of two tiles
    private IEnumerator SmoothSwap(Tile tileA, Tile tileB)
    {
        float duration = 0.1f;
        float elapsedTime = 0f;

        Vector3 startPositionA = tileA.transform.localPosition;
        Vector3 startPositionB = tileB.transform.localPosition;

        while (elapsedTime < duration)
        {
            tileA.transform.localPosition = Vector3.Lerp(startPositionA, startPositionB, elapsedTime / duration);
            tileB.transform.localPosition = Vector3.Lerp(startPositionB, startPositionA, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tileA.transform.localPosition = startPositionB;
        tileB.transform.localPosition = startPositionA;

        int tileAIndex = tiles.IndexOf(tileA);
        int tileBIndex = tiles.IndexOf(tileB);
        tiles[tileAIndex] = tileB;
        tiles[tileBIndex] = tileA;
    }

    // Checks if the level is complete by comparing tile positions with expected positions
    private void CheckLevelCompletion()
    {
        string currentOrder = "";

        for (int i = 0; i < 15; i++)
        {
            currentOrder += tiles[i].tileNumber + " ";
            if (tiles[i].tileNumber != i + 1)
            {
                Debug.Log("Current order: " + currentOrder);
                return;
            }
        }
        donePanel.SetActive(true);
    }

    // Gets the grid position of a tile based on its position in the list
    public Vector3Int GetTileGridPosition(Tile tile)
    {
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                if (tiles[y * gridSize + x] == tile)
                {
                    return new Vector3Int(x, y, 0);
                }
            }
        }
        return new Vector3Int(-1, -1, -1);
    }

    // Gets the tile at a specific grid position
    private Tile GetTileAtPosition(int x, int y)
    {
        return tiles[y * gridSize + x];
    }
}