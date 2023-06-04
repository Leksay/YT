using UnityEngine;

public static class Vector3Ex
{
    public static Vector3 ToXZVector(this Vector2 xyVector) => new Vector3(xyVector.x, 0f, xyVector.y);
    public static Vector2 FromXZ(this Vector3 vector) => new Vector2(vector.x, vector.z);
}