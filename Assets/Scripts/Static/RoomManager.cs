using System;
using System.Linq;
using UnityEngine;

public static class RoomManager
{
    public static Room[] Rooms { get; set; }
    public static Room ActiveRoom { get; set; }

    public static void CacheRooms()
    {
        Rooms = GameObject.FindGameObjectsWithTag("Room")
            .Select(obj => obj.GetComponent<Room>())
            .ToArray();

        foreach (Room room in Rooms)
        {
            if (PlayerWithinBounds(room))
            {
                ActiveRoom = room;
            }
        }
    }

    public static void LoadRooms()
    {
        foreach (Room room in Rooms)
        {
            LoadRoom(room, room == ActiveRoom || ActiveRoom.ConnectedRooms.Contains(room));
        }
    }

    static void LoadRoom(Room room, bool yes)
    {

    }


    public static bool PlayerWithinBounds(Room room)
    {
        Vector2 _playerPos = PlayerConfig.Instance.transform.position;
        Vector2 _mapPos = room.transform.position;
        float _width = room.Size.x;
        float _height = room.Size.y;

        float _top = _mapPos.y + _height / 2;
        float _bottom = _mapPos.y - _height / 2;
        float _right = _mapPos.x + _width / 2;
        float _left = _mapPos.x - _width / 2;

        return
            _playerPos.x <= _right &&
            _playerPos.x >= _left &&
            _playerPos.y <= _top &&
            _playerPos.y >= _bottom
        ;
    }
}
