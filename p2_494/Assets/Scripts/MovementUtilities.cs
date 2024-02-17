using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MovementUtilities
{
    public static void SnapPosition(GameObject obj, float movementUnit)
    {
        float roundedX = RoundToNearestGrid(obj.transform.position.x, movementUnit);
        float roundedY = RoundToNearestGrid(obj.transform.position.y, movementUnit);
        obj.transform.position = new Vector3(roundedX,
                                         roundedY,
                                         obj.transform.position.z);
    }

    static float RoundToNearestGrid(float pos, float gridSize)
    {
        float xDiff = pos % gridSize;
        bool isPositive = pos > 0 ? true : false;
        pos -= xDiff;
        if (Mathf.Abs(xDiff) > (gridSize / 2))
        {
            if (isPositive)
            {
                pos += gridSize;
            }
            else
            {
                pos -= gridSize;
            }
        }
        return pos;
    }

}
