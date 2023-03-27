using UnityEngine;
using System;

public enum Direction2D
{
    North,
    South,
    East,
    West,
    Default
}

public static class Direction2DExtensions
{
    public static Vector2 ToVector2(this Direction2D direction)
    {
        switch (direction)
        {
            case Direction2D.North:
                return new Vector2(0, 1);
            case Direction2D.South:
                return new Vector2(0, -1);
            case Direction2D.East:
                return new Vector2(1, 0);
            case Direction2D.West:
                return new Vector2(-1, 0);
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, "Invalid direction value.");
        }
    }

    public static Direction2D GetRandomDirection()
    {
        Array values = Enum.GetValues(typeof(Direction2D));
        System.Random random = new System.Random();
        Direction2D randomDirection = (Direction2D)values.GetValue(random.Next(values.Length));
        return randomDirection;
    }
}
