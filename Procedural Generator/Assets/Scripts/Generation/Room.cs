using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPrefabsContainer
{
    public GameObject room;
    public GameObject door;
    public GameObject wall;

    public RoomPrefabsContainer(GameObject room, GameObject door, GameObject wall)
    {
        this.room = room;
        this.door = door;
        this.wall = wall;
    }
}

public class Room : MonoBehaviour
{
    private RoomPrefabsContainer roomPrefabs;

    List<PlacedRoom> roomsToBuild = new List<PlacedRoom>();

    private Transform parent;
    private GameObject thisRoom;

    public void Build(List<PlacedRoom> roomsToBuild, RoomPrefabsContainer roomPrefabs, GameObject parent)
    {
        // Clear all used lists
        this.roomsToBuild.Clear();

        // Store the matrix for later use
        this.roomsToBuild = roomsToBuild;

        // Remember prefabs
        this.roomPrefabs = roomPrefabs;

        // Remember parent
        this.parent = parent.transform;

        PlacePrefab();

        Destroy(this.gameObject);
    }

    void PlacePrefab()
    {
        GameObject newRoom = Instantiate(roomPrefabs.room, this.transform.position, Quaternion.identity);
        newRoom.name = "Finished Room";
        newRoom.transform.parent = parent;
        thisRoom = newRoom;

        foreach (Direction2D direction in Direction2D.GetValues(typeof(Direction2D)).Cast<Direction2D>().Where(d => d != Direction2D.Default))
        {
            if (IsRoomTowardsDirection(direction))
            {
                PlaceDoor(direction);
            }
            else PlaceWall(direction);
        }
    }

    void PlaceDoor(Direction2D direction)
    {
        GameObject door = Instantiate(roomPrefabs.door, this.gameObject.transform.position, Quaternion.identity);
        door.transform.parent = thisRoom.transform;
        RotateAndMoveTowardsDirection(door, direction);
    }

    void PlaceWall(Direction2D direction)
    {
        GameObject wall = Instantiate(roomPrefabs.wall, this.gameObject.transform.position, Quaternion.identity);
        wall.transform.parent = thisRoom.transform;
        RotateAndMoveTowardsDirection(wall, direction);
    }

    void RotateAndMoveTowardsDirection(GameObject objectToRotate, Direction2D direction)
    {
        float objectX = 0f;
        float objectY = 0f;
        Quaternion objectRotation = Quaternion.identity;

        switch (direction)
        {
            case Direction2D.North:
                objectY = (this.gameObject.transform.localScale.y / 2f);
                objectRotation = Quaternion.Euler(0f, 0f, 180f);
                break;
            case Direction2D.East:
                objectX = (this.gameObject.transform.localScale.x / 2f);
                objectRotation = Quaternion.Euler(0f, 0f, -270);
                break;
            case Direction2D.South:
                objectY = -(this.gameObject.transform.localScale.y / 2f);
                objectRotation = Quaternion.Euler(0f, 0f, 0f);
                break;
            case Direction2D.West:
                objectX = -(this.gameObject.transform.localScale.x / 2f);
                objectRotation = Quaternion.Euler(0f, 0f, -90f);
                break;
            default:
                Debug.LogError("Invalid direction: " + direction);
                return;
        }

        objectToRotate.name += " (" + direction + ")";
        objectToRotate.transform.position = this.gameObject.transform.position + new Vector3(objectX, objectY, 0f);
        objectToRotate.transform.rotation = objectRotation;
    }

    bool IsRoomTowardsDirection(Direction2D direction)
    {
        Vector2 directionVector = direction.ToVector2();
        float directionX = directionVector.x * this.transform.localScale.x;
        float directionY = directionVector.y * this.transform.localScale.y;

        Vector2 currentPosition = new Vector2(this.transform.position.x, this.transform.position.y);

        Vector2 directionPosition = currentPosition + new Vector2(directionX, directionY);

        if (IsPlacedRoomAtPosition(directionPosition))
        {
            return true;
        }
        else return false;
    }

    private bool IsPlacedRoomAtPosition(Vector2 position)
    {
        foreach (PlacedRoom placedRoom in roomsToBuild)
        {
            if (placedRoom.position == position)
            {
                return true;
            }
        }

        return false;
    }
}
