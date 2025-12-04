using UnityEngine;

public enum Direction
{
    None,
    Up,
    Right,
    Down,
    Left,
}

public class Trigger : MonoBehaviour
{
    enum Axis
    {
        Horizontal,
        Vertical,
    }

    [SerializeField] Axis axis = Axis.Horizontal;
    protected Direction _lastDirection = Direction.None;

    protected Direction OppositeDirection(Direction direction)
    {
        return direction switch
        {
            Direction.Right => Direction.Left,
            Direction.Left => Direction.Right,
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            _ => Direction.None,
        };
    }

    protected Direction FromDirection(Collider2D col)
    {
        Vector2 _pos = transform.position;
        Vector2 _playerPos = col.transform.position;

        if (axis == Axis.Horizontal)
        {
            float _xDistance = _playerPos.x - _pos.x;
            return _xDistance > 0 ? Direction.Right : Direction.Left;
        }
        else if (axis == Axis.Vertical)
        {
            float _yDistance = _playerPos.y - _pos.y;
            return _yDistance > 0 ? Direction.Up : Direction.Down;
        }

        return Direction.None;
    }
}
