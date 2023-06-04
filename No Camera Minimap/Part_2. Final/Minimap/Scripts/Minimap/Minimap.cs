using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Minimap : MonoBehaviour
{
    #region constants

    private const float DEFAULT_SCALE_STEP = 0.25f;

    #endregion

    #region fields

    [SerializeField] private Rect _worldMapRect;
    [SerializeField] private RectTransform _iconsRoot;
    [SerializeField] private RectTransform _rootUi;
    [SerializeField] private MapAgentGraphics _agentGraphicsPrefab;

    [Header("Scale")] 
    [SerializeField] private Button _scaleUp;
    [SerializeField] private Button _scaleDown;
    [SerializeField] private float _scaleStep;

    private List<IMinimapAgent> _agents = new();
    private Dictionary<IMinimapAgent, MapAgentGraphics> _agentToGraphicsMap = new();

    private IMinimapAgent _playerAgent;
    private RectTransform _playerRect;
    private RectTransform _rectTransform;
    private Vector2 _mapOffset;
    private Rect _currentUiRect;

    private float _mapUiBorder;
    private float _mapUIHalfSize;
    private float _mapScale;

    #endregion

    #region engine methods

    private void Awake()
    {
        _rectTransform = (RectTransform)transform;
        _mapScale = _rectTransform.localScale.x;

        CalculateBorders();

        if (_scaleUp)
            _scaleUp.onClick.AddListener(ScaleUp);

        if (_scaleDown)
            _scaleDown.onClick.AddListener(ScaleDown);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.KeypadPlus))
            ScaleUp();

        if (Input.GetKeyUp(KeyCode.KeypadMinus))
            ScaleDown();
    }

    private void FixedUpdate()
    {
        if (_playerRect)
            SetPlayerRelativeOffset();

        _currentUiRect = _rectTransform.rect;

        foreach (IMinimapAgent agent in _agents)
            UpdateAgent(agent);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_worldMapRect.center.ToXZVector(), _worldMapRect.size.ToXZVector());
    }

    #endregion

    #region public methods

    public void SetData(Vector3 startPointPosition, Vector3 endPointPosition, RectTransform iconsRoot,
        RectTransform mapRoot)
    {
        _worldMapRect = MapUtils.CalculateBorders(startPointPosition, endPointPosition);
        _iconsRoot = iconsRoot;
        _rootUi = mapRoot;
        _scaleStep = DEFAULT_SCALE_STEP;
    }

    public void RegisterPlayer(IMinimapAgent agent)
    {
        if (_playerRect)
        {
            Debug.LogError("Player already registered in minimap");
            return;
        }

        Register(agent);

        _playerRect = _agentToGraphicsMap[agent].RectTransform;
        _playerAgent = agent;
    }

    public void UnregisterPlayer(IMinimapAgent agent)
    {
        if (!_playerRect || _agentToGraphicsMap[agent].RectTransform != _playerRect)
        {
            Debug.LogError($"Can't unregister player {agent}");
            return;
        }

        _playerRect = null;
        _playerAgent = null;

        Unregister(agent);
    }

    public void Register(IMinimapAgent agent)
    {
        if (!_agents.Contains(agent))
        {
            CreateAgentGraphics(agent);
            _agents.Add(agent);
        }
    }

    public void Unregister(IMinimapAgent agent)
    {
        if (_agents.Remove(agent))
            DeleteAgentGraphics(agent);
    }

    #endregion

    #region service methods

    private void CreateAgentGraphics(IMinimapAgent agent)
    {
        MapAgentGraphics agentGraphics = Instantiate(_agentGraphicsPrefab, _iconsRoot);
        _agentToGraphicsMap.Add(agent, agentGraphics);

        agentGraphics.Initialize(agent.MapIcon, agent.IconColor, agent.Name);
    }

    private void DeleteAgentGraphics(IMinimapAgent agent)
    {
        if (_agentToGraphicsMap.TryGetValue(agent, out MapAgentGraphics graphics))
        {
            if (graphics)
                Destroy(graphics.gameObject);

            _agentToGraphicsMap.Remove(agent);
        }
    }

    private void SetPlayerRelativeOffset()
    {
        Vector2 nextOffset = GetRemappedOffset();

        if (_mapUiBorder - Mathf.Abs(nextOffset.x) < 0)
            nextOffset.x = _mapUiBorder * Mathf.Sign(nextOffset.x);

        if (_mapUiBorder - Mathf.Abs(nextOffset.y) < 0)
            nextOffset.y = _mapUiBorder * Mathf.Sign(nextOffset.y);

        _mapOffset = nextOffset;
        _rectTransform.anchoredPosition = _mapOffset;
    }

    private void UpdateAgent(IMinimapAgent agent)
    {
        MapAgentGraphics graphics = _agentToGraphicsMap[agent];
        Vector2 xzPosition = agent.Pose.position.FromXZ();

        float zAngle = agent.Pose.rotation.eulerAngles.y;

        Vector2 localPosition =
            _mapScale * MapUtils.GetRemappedRectPosition(_worldMapRect, _currentUiRect, xzPosition);

        graphics.RectTransform.anchoredPosition = _mapOffset + localPosition;
        graphics.RectTransform.localRotation = Quaternion.Euler(0f, 0f, -zAngle);
    }

    private void ScaleUp()
    {
        _mapScale += _scaleStep;
        _rectTransform.localScale = Vector3.one * _mapScale;

        CalculateBorders();
        SetPlayerRelativeOffset();
    }

    private void ScaleDown()
    {
        if (_rectTransform.rect.width * (_mapScale - 0.25f) < _rectTransform.rect.width)
            return;

        _mapScale -= _scaleStep;
        _rectTransform.localScale = Vector3.one * _mapScale;

        CalculateBorders();
        SetPlayerRelativeOffset();
    }

    private void CalculateBorders()
        => _mapUiBorder = _rectTransform.rect.width * _mapScale / 2 - _rootUi.rect.width / 2;

    private Vector2 GetRemappedOffset()
    {
        return -1 * _mapScale *
               MapUtils.GetRemappedRectPosition(_worldMapRect, _currentUiRect, _playerAgent.Pose.position.FromXZ());
    }

    #endregion
}