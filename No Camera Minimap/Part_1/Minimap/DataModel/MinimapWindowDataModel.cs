using UnityEngine;

public class MinimapWindowDataModel
{
    public Vector3Model StartPosition;
    public Vector3Model EndPosition;
}

[System.Serializable]
public struct Vector3Model
{
    public float X;
    public float Y;
    public float Z;

    public Vector3 ToVector3() => new Vector3(X, Y, Z);

    public static Vector3Model FromVector3(Vector3 vector3)
    {
        return new Vector3Model()
        {
            X = vector3.x,
            Y = vector3.y,
            Z = vector3.z
        };
    }
}