using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBuilder : MonoBehaviour
{
    [SerializeField]
    private GameObject roomPrefab, doorPrefab, wallPrefab;

    private List<PlacedRoom> placedRooms = new List<PlacedRoom>();

    private void OnEnable()
    {
        // Get an instance of the RoomPositioner script in the scene
        RoomPositioner roomPositioner = FindObjectOfType<RoomPositioner>();

        // Subscribe to the OnRoomsPlaced event of the RoomPositioner script
        roomPositioner.OnRoomsPlaced += StartBuilding;
    }

    private void OnDisable()
    {
        // Get an instance of the RoomPositioner script in the scene
        RoomPositioner roomPositioner = FindObjectOfType<RoomPositioner>();

        // Unsubscribe from the OnRoomsPlaced event when this script is destroyed
        if (roomPositioner != null)
        {
            roomPositioner.OnRoomsPlaced -= StartBuilding;
        }
    }

    private void StartBuilding(List<PlacedRoom> placedRooms)
    {
      // Clear all used lists
        this.placedRooms.Clear();

        // Store the matrix for later use
        this.placedRooms = placedRooms;

        BuildRooms();
    }

    private void BuildRooms()
    {
        // Create the parent for all the rooms
        GameObject parent = new GameObject();
        parent.name = "Rooms";

        foreach (PlacedRoom room in placedRooms)
        {
            room.gameObject.AddComponent<Room>();

            RoomPrefabsContainer prefabsContainer = new RoomPrefabsContainer(roomPrefab,
                                                                                doorPrefab,
                                                                                wallPrefab);

            room.gameObject.GetComponent<Room>().Build(placedRooms, prefabsContainer, parent);
        }
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
}
