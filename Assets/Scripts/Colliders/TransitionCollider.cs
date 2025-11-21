using System;
using Unity.VisualScripting;
using UnityEngine;



[System.Serializable]
public struct TransitionConnection
{
    public Direction origin;
    public Room room;
}

public class TransitionCollider : Trigger
{
    [SerializeField] Camera mainCamera;
    [SerializeField] TransitionConnection connectA;
    [SerializeField] TransitionConnection connectB;
    //[SerializeField] Vector2 translateDistance;



    void Start()
    {
        mainCamera = Camera.main;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Direction fromDirection = FromDirection(other);
        lastDirection = fromDirection;
        //Debug.Log($"Enter: {fromDirection}");


        MoveCamera(OppositeDirection(fromDirection));
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Room newRoom = RoomManager.Instance.GetActiveRoom();
        //Debug.Log("New room position: " + newRoom.transform.position);

        Direction fromDirection = FromDirection(other);
        if (fromDirection != lastDirection)
        {
            lastDirection = Direction.None;
            return;
        }

        Debug.Log($"Exit: {fromDirection}");

        MoveCamera(fromDirection);
        lastDirection = Direction.None;
    }

    void MoveCamera(Direction direction)
    {
        Room targetRoom = TargetRoom(direction);
        RoomManager.Instance.SetActiveRoom(targetRoom);
        /*
                Vector2 translate = DirectionToVector(direction);
                Vector3 pos = mainCamera.transform.position;
                Vector3 newPos = new(pos.x + translate.x, pos.y + translate.y, pos.z); */

        CameraMove.Instance.MoveToRoom(targetRoom);
    }

    Room TargetRoom(Direction direction)
    {
        try
        {
            if (connectA.origin == direction) return connectA.room;
            else if (connectB.origin == direction) return connectB.room;

            throw new SystemException("No room connection for direction: " + direction);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    /*     Vector2 DirectionToVector(Direction direction)
        {
            return direction switch
            {
                Direction.Right => Vector2.right * translateDistance.x,
                Direction.Left => Vector2.left * translateDistance.x,
                Direction.Up => Vector2.up * translateDistance.y,
                Direction.Down => Vector2.down * translateDistance.y,
                _ => Vector2.zero,
            };
        } */


}
