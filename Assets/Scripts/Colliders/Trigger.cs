using UnityEngine;

public enum Direction
{
    None,
    Up,
    Right,
    Down,
    Left,
}
public enum Axis
{
    Horizontal,
    Vertical,
}

public class Trigger : MonoBehaviour
{
    [SerializeField] Axis axis = Axis.Horizontal;
    protected Direction lastDirection = Direction.None;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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

    protected Direction FromDirection(Collider2D playerCol)
    {
        Vector2 pos = transform.position;
        Vector2 playerPos = playerCol.transform.position;

        if (axis == Axis.Horizontal)
        {
            float xDistance = playerPos.x - pos.x;
            return xDistance > 0 ? Direction.Right : Direction.Left;
        }
        else if (axis == Axis.Vertical)
        {
            float yDistance = playerPos.y - pos.y;
            return yDistance > 0 ? Direction.Up : Direction.Down;
        }

        return Direction.None;
    }
}
