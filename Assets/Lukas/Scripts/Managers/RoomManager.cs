using System.Linq;
using System;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class RoomManager : Singleton<RoomManager>
{
    public Room[] rooms;
    public Room activeRoom;
    void Start()
    {
        rooms = GameObject.FindGameObjectsWithTag("Room")
            .Select(obj => obj.GetComponent<Room>())
            .ToArray();

    }

    bool PlayerWithinBounds(Room room)
    {
        Vector2 playerPos = PlayerConfig.Instance.transform.position;
        Vector2 mapPos = room.transform.position;
        float width = room.size.x;
        float height = room.size.y;

        float top = mapPos.y + height / 2;
        float bottom = mapPos.y - height / 2;
        float right = mapPos.x + width / 2;
        float left = mapPos.x - width / 2;

        return
            playerPos.x <= right &&
            playerPos.x >= left &&
            playerPos.y <= top &&
            playerPos.y >= bottom
        ;
    }

    public Room GetActiveRoom()
    {
        try
        {
            foreach (Room room in rooms)
            {
                if (PlayerWithinBounds(room)) return room;
            }
            throw new Exception("Active room could not be found");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            Debug.LogError(
                "Player position: "
                + PlayerConfig.Instance.transform.position
                + " Rooms: "
                + rooms.Length
            );
            return null;
        }


    }

    public void SetActiveRoom(Room room)
    {
        activeRoom = room;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
