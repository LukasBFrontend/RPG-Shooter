using System;
using UnityEngine;

public class TransitionCollider : Trigger
{
    [Serializable]
    struct TransitionConnection
    {
        public Direction Origin;
        public Room Room;
    }

    [SerializeField] TransitionConnection connectA;
    [SerializeField] TransitionConnection connectB;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Direction _fromDirection = FromDirection(other);
        _lastDirection = _fromDirection;

        Room targetRoom = TargetRoom(OppositeDirection(_fromDirection));
        RoomManager.ActiveRoom = targetRoom;
        CameraMove.Instance.MoveToRoom(targetRoom);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Direction fromDirection = FromDirection(other);
        if (fromDirection != _lastDirection)
        {
            _lastDirection = Direction.None;
            return;
        }

        Room targetRoom = TargetRoom(fromDirection);
        RoomManager.ActiveRoom = targetRoom;
        CameraMove.Instance.MoveToRoom(targetRoom);

        _lastDirection = Direction.None;
    }

    Room TargetRoom(Direction direction)
    {
        try
        {
            if (connectA.Origin == direction) return connectA.Room;
            else if (connectB.Origin == direction) return connectB.Room;

            throw new SystemException("No room connection for direction: " + direction);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
}
