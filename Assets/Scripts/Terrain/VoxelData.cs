using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelData {

    public static Vector3[] HexVerticies(float radius) {
        Vector3[] points = new Vector3[6];
        for (int i = 0; i < 6; i++) {
            float angleDegrees = 60 * i;
            float angleRadians = Mathf.PI / 180 * angleDegrees;
            Vector3 xzPoint = new Vector3(radius * Mathf.Cos(angleRadians), 0, radius * Mathf.Sin(angleRadians));
            points[i] = new Vector3(xzPoint.x, 0, xzPoint.z);
        }
        return points;
    }

    public readonly static int[] hexTriangles = new int[] {
        5, 4, 0,
        4, 3, 0,
        3, 2, 0,
        2, 1, 0,
    };

    public readonly static Vector2Int[] hexNeighborDirection = new Vector2Int[] {
        new Vector2Int(1,0),
        new Vector2Int(-1,0),
        new Vector2Int(0,1),
        new Vector2Int(0,-1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,1)
    };

    public readonly static int[] hexNeigborVerticies = new int[] {
        1, 2, 4, 5,
        4, 5, 1, 2,
        0, 1, 3, 4,
        3, 4, 0, 1,
        2, 3, 5, 0,
        5, 0, 2, 3
    };

    public readonly static int[] hexNeighborTriangles = new int[] {
        0, 1, 2,
        0, 2, 3
    };
}

