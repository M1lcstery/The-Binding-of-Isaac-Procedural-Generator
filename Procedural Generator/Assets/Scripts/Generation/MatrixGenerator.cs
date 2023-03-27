using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrixRoom
{
    public GameObject gameObject;
    public Vector2 position;
}

public class MatrixGenerator : MonoBehaviour
{
    // Variables

    [Header("Sizes")]

    [SerializeField, Min(1)]
    private int sizeX = 0;
    [SerializeField, Min(1)]
    private int sizeY = 0;

    [Space(10)]

    [Header("Prefabs")]
    [SerializeField]
    private GameObject matrixPrefab;

    private List<MatrixRoom> matrixRooms = new List<MatrixRoom>();

    // Events

    public delegate void MatrixCreatedEventHandler(List<MatrixRoom> matrixRooms);
    public event MatrixCreatedEventHandler OnMatrixCreated;

    public void Start()
    {
        Run();
    }

    public void Run()
    {
        CreateMatrix(sizeX, sizeY);
    }

    private void CreateMatrix(int X, int Y)
    {
        // Main Loop

        for (int i = 0; i < X; i++)
        {
            for (int j = 0; j < Y; j++)
            {
                Vector2 position = new Vector2(i, j);

                CreateMatrixRoom(position);
            }
        }

        // Raise the OnMatrixCreated event and pass the matrix as an argument
        OnMatrixCreated?.Invoke(matrixRooms);
    }

    private void ClearMatrix()
    {
        // Searches for each GameObject in the matrix list and destroys it
        // Then clears the list

        foreach (MatrixRoom room in matrixRooms)
        {
            Destroy(room.gameObject);
        }

        matrixRooms.Clear();
    }

    private void CreateMatrixRoom(Vector2 position)
    {
        // First, we instantiate the newRoom GameObject, then we add it to the matrix
        // After that, we name it and make it a child of this GameObject

        // Adjusting sizes so everything is represented correctly in the matrix
        float roomSizeX = matrixPrefab.transform.localScale.x;
        float roomSizeY = matrixPrefab.transform.localScale.y;

        position = new Vector2(position.x * roomSizeX, position.y * roomSizeY);

        BuildMatrixRoom(position);
    }

    private void BuildMatrixRoom(Vector2 position)
    {
        // Instantiate and handle the GameObject
        GameObject newRoom = Instantiate(matrixPrefab, position, Quaternion.identity);
        newRoom.name = "Room " + new Vector2(newRoom.transform.position.x, newRoom.transform.position.y);
        newRoom.transform.parent = this.transform;

        // Instantiate and handle the MatrixRoom
        MatrixRoom newMatrixRoom = new MatrixRoom();
        newMatrixRoom.gameObject = newRoom;
        newMatrixRoom.position = newRoom.transform.position;
        matrixRooms.Add(newMatrixRoom);
    }
}
