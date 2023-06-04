using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MapAgentGraphics : MonoBehaviour
{
    public RectTransform RectTransform { get; private set; }
    private Image _icon;

    private void Awake()
    {
        RectTransform = (RectTransform)transform;
        _icon = GetComponent<Image>();
    }

    public void Initialize(Sprite iconSprite, Color color, string agentName = null)
    {
        if (iconSprite)
            _icon.sprite = iconSprite;

        if (!string.IsNullOrEmpty(agentName))
            gameObject.name = agentName;

        _icon.color = color;
    }
}