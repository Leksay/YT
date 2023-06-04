using UnityEngine;

public interface IMinimapAgent
{
    Pose Pose { get; }
    Sprite MapIcon { get; }
    string Name { get; }
    Color IconColor { get; }
}