using UnityEngine;

public static class MapUtils
{
    public static Rect CalculateBorders(Vector3 start, Vector3 end)
    {
        float minX, maxX, minZ, maxZ;

        if (start.x > end.x)
        {
            minX = end.x;
            maxX = start.x;
        }
        else
        {
            minX = start.x;
            maxX = end.x;
        }

        if (start.z > end.z)
        {
            minZ = end.z;
            maxZ = start.z;
        }
        else
        {
            maxZ = end.z;
            minZ = start.z;
        }

        float sizeX = maxX - minX;
        float sizeZ = maxZ - minZ;

        return new Rect(minX, minZ, sizeX, sizeZ);
    }

    public static void CalculateSubRects(out Rect[] rects, int subdivideCount, Rect borders)
    {
        float rectWidth = (borders.xMax - borders.xMin) / subdivideCount;
        float rectDepth = (borders.yMax - borders.yMin) / subdivideCount;

        rects = new Rect[subdivideCount * subdivideCount];

        for (int x = 0; x < subdivideCount; x++)
        {
            float startX = borders.xMin + x * rectWidth;

            for (int z = 0; z < subdivideCount; z++)
            {
                float startZ = borders.yMin + z * rectDepth;

                Rect rect = new Rect(startX, startZ, rectWidth, rectDepth);

                rects[x * subdivideCount + z] = rect;
            }
        }
    }

    public static float Remap(float min1, float max1, float min2, float max2, float value) 
        => min2 + (value - min1) * (max2 - min2) / (max1 - min1);

    public static Vector2 GetRemappedRectPosition(Rect from, Rect to, Vector2 position)
    {
        float x = Remap(from.xMin, from.xMax, to.xMin, to.xMax, position.x);
        float y = Remap(from.yMin, from.yMax, to.yMin, to.yMax, position.y);

        return new Vector2(x, y);
    }
}