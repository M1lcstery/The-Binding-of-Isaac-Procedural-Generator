using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedRoom
{
    public GameObject gameObject;
    public Vector2 position;
}

public class RoomPositioner : MonoBehaviour
{
    [SerializeField]
    private GameObject roomPrefab;

    [SerializeField, Min(1)]
    private int roomsToPlace;

    // Events

    public delegate void RoomsPlacedEventHandler(List<PlacedRoom> placedRooms);
    public event RoomsPlacedEventHandler OnRoomsPlaced;

    // Runtime

    private List<PlacedRoom> placedRooms = new List<PlacedRoom>();
    private List<MatrixRoom> matrixRooms = new List<MatrixRoom>();

    private Vector2 startingPosition;

    private void OnEnable()
    {
        // Get an instance of the MatrixGenerator script in the scene
        MatrixGenerator matrixGenerator = FindObjectOfType<MatrixGenerator>();

        // Subscribe to the OnMatrixCreated event of the MatrixGenerator script
        matrixGenerator.OnMatrixCreated += StartPositioning;
    }

    private void OnDisable()
    {
        // Get an instance of the MatrixGenerator script in the scene
        MatrixGenerator matrixGenerator = FindObjectOfType<MatrixGenerator>();

        // Unsubscribe from the OnMatrixCreated event when this script is destroyed
        if (matrixGenerator != null)
        {
            matrixGenerator.OnMatrixCreated -= StartPositioning;
        }
    }

    private void StartPositioning(List<MatrixRoom> matrix)
    {
        // Clear all used lists

        this.matrixRooms.Clear();
        placedRooms.Clear();

        // Store the matrix for later use
        this.matrixRooms = matrix;

        // Get starting position for this matrix
        GetStartingPosition(matrix);

        // Testing
        // TODO: Receive room quantity

        PlaceRooms(roomsToPlace);
    }

    // Dungeon Generation

    private Vector2 GetStartingPosition(List<MatrixRoom> matrix)
    {
        int numRows = Mathf.FloorToInt(Mathf.Sqrt(matrix.Count));
        int numCols = Mathf.CeilToInt((float)matrix.Count / numRows);

        if (matrix.Count % 2 == 0)
        {
            int middleRowIndex = numRows / 2;
            int middleColIndex = numCols / 2;

            // Pick one of the four middle squares randomly

            int[] rowIndices = { middleRowIndex - 1, middleRowIndex };
            int[] colIndices = { middleColIndex - 1, middleColIndex };

            int randomRowIndex = Random.Range(0, 2);
            int randomColIndex = Random.Range(0, 2);

            int middleIndex = rowIndices[randomRowIndex] * numCols + colIndices[randomColIndex];

            startingPosition = matrix[middleIndex].position;
        }
        else
        {
            int middleIndex = Mathf.FloorToInt(matrix.Count / 2);

            startingPosition = matrix[middleIndex].position;
        }

        return startingPosition;
    }

    private void PlaceRooms(int roomsToPlace)
    {
        Vector2 lastRoomPosition = new Vector2();

        while (roomsToPlace > 0)
        {
            roomsToPlace--;
            // Place rooms
            if (!(placedRooms.Count >= matrixRooms.Count))
            {
                lastRoomPosition = PlaceRoom(startingPosition);
            }
        }

        // Raise the event with the list of placed rooms

        OnRoomsPlaced?.Invoke(placedRooms);

    }

    private Vector2 PlaceRoom(Vector2 lastRoomPosition)
    {
        Vector2 roomPosition = new Vector2();
        roomPosition = GetRoomPosition(lastRoomPosition);

        GameObject newRoom = BuildRoom(roomPosition);

        // Return newRoom position for the next room
        return newRoom.transform.position;
    }

    private GameObject BuildRoom(Vector2 roomPosition)
    {
        // Instantiate and handle the GameObject
        GameObject newRoom = Instantiate(roomPrefab, roomPosition, Quaternion.identity);
        newRoom.transform.parent = this.transform;
        newRoom.name = "Room " + placedRooms.Count;

        // Instantiate and handle the PlacedRoom
        PlacedRoom newPlacedRoom = new PlacedRoom();
        newPlacedRoom.gameObject = newRoom;
        newPlacedRoom.position = newRoom.transform.position;
        placedRooms.Add(newPlacedRoom);
        return newRoom;
    }

    // Positioning Methods

    private Vector2 GetRoomPosition(Vector2 lastRoomPosition)
    {
        // Check if lastRoomPosition is possible
        if (IsRoomPositionValid(lastRoomPosition))
        {
            return lastRoomPosition;
        }

        List<Vector2> possiblePositions = new List<Vector2>();

        // Check for empty positions in the matrix for possible positions
        foreach (Direction2D direction in Direction2D.GetValues(typeof(Direction2D)).Cast<Direction2D>().Where(d => d != Direction2D.Default))
        {
            Vector2 directionVector = direction.ToVector2();
            Vector2 directionPosition = lastRoomPosition + new Vector2(directionVector.x * roomPrefab.transform.localScale.x, directionVector.y * roomPrefab.transform.localScale.y);

            if (IsRoomPositionValid(directionPosition))
            {
                possiblePositions.Add(directionPosition);
            }
        }

        // If any possible position is found, return it
        if (possiblePositions.Count > 0)
        {
            return possiblePositions[Random.Range(0, possiblePositions.Count)];
        }
        else
        {
            // Find another starting point and run this same method
            return GetRoomPosition(placedRooms[Random.Range(0, placedRooms.Count)].position);
        }
    }

    // Conditions and utilities

    private bool IsRoomPositionValid(Vector2 roomPosition)
    {
        if (IsMatrixRoomAtPosition(roomPosition) && !IsPlacedRoomAtPosition(roomPosition))
        {
            return true;
        }

        return false;
    }

    private bool IsPlacedRoomAtPosition(Vector2 position)
    {
        foreach (PlacedRoom placedRoom in placedRooms)
        {
            if (placedRoom.position == position)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsMatrixRoomAtPosition(Vector2 position)
    {
        foreach (MatrixRoom matrixRoom in matrixRooms)
        {
            if (matrixRoom.position == position)
            {
                return true;
            }
        }

        return false;
    }
}