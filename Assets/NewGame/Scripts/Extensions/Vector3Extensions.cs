using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 Set(this Vector3 vector, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
    }

    public static Vector3 Add(this Vector3 vector, float? x = null, float? y = null, float? z = null)
    {
        return new Vector3(vector.x + (x ?? 0), vector.y + (y ?? 0), vector.z + (z ?? 0));
    }

    public static Vector3 Add(this Vector3 vector, Vector3 other)
    {
        return new Vector3(vector.x + other.x, vector.y + other.y, vector.z + other.z);
    }
}