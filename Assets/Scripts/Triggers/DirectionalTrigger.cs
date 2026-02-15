using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public enum Direction
{
    None,
    Up,
    Right,
    Down,
    Left,
}

public static class DirectionExtensions
{
    static readonly Dictionary<Vector2, Direction> _directionLookUp = new()
    {
        [Vector2.zero] = Direction.None,
        [Vector2.up] = Direction.Up,
        [Vector2.right] = Direction.Right,
        [Vector2.down] = Direction.Down,
        [Vector2.left] = Direction.Left,
    };

    static readonly Dictionary<Direction, Vector2> _vectorLookUp = _directionLookUp.ToDictionary(x => x.Value, x => x.Key);
    public static Vector2 ToVector(this Direction direction)
    {
        return _vectorLookUp[direction];
    }

    public static Direction FromVector(Vector2 vector)
    {
        return _directionLookUp[vector];
    }

    public static Direction Opposite(this Direction direction)
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
}

public enum Axis
{
    None,
    Horizontal,
    Vertical,
}

public static class AxisExtensions
{
    public static Vector2 ToVector(this Axis axis)
    {
        return axis switch
        {
            Axis.None => new(0, 0),
            Axis.Horizontal => new(1, 0),
            Axis.Vertical => new(0, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
        };
    }
}

public class DirectionalTrigger : MonoBehaviour
{

    [SerializeField] new BoxCollider2D collider;
    [SerializeField] Axis axis = Axis.None;
    public BoxCollider2D Collider { get => collider; }
    public Axis Axis { get => axis; }

    public Direction ApproachDirection(Collider2D col)
    {

        Vector3 _intersect = collider.bounds.ClosestPoint(col.transform.position);
        Vector2 _relative = _intersect - collider.bounds.center;

        Vector2 _dir = new Vector2(
            Mathf.Sign(_relative.x),
            Mathf.Sign(_relative.y)
        )
            * axis.ToVector()
        ;

        return DirectionExtensions.FromVector(_dir);
    }
}
