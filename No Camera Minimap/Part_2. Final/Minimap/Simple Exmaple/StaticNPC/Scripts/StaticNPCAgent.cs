using UnityEngine;

public class StaticNPCAgent : MonoBehaviour, IMinimapAgent
{
    #region IMinimapAgent

    public Pose Pose => new Pose(transform.position, transform.rotation);

    [field: SerializeField]
    public Sprite MapIcon { get; private set; }

    [field: SerializeField]
    public string Name { get; private set;}

    [field: SerializeField]
    public Color IconColor { get; private set;}

    #endregion
}
