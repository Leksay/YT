using System.Linq;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Only for example purposes
/// </summary>
public class MinimapAgentRegistrator : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private List<GameObject> _minimapAgents;
    [SerializeField] private Minimap _minimap;

    private void OnValidate()
    {
        if (_minimapAgents != null && _minimapAgents.Count > 0)
        {
            for (int i = _minimapAgents.Count - 1; i >= 0; i--)
            {
                if(!_minimapAgents[i] || _minimapAgents[i].GetComponent<IMinimapAgent>() == null)
                    _minimapAgents.RemoveAt(i);
            }   
        }

        if (!_player || _player.GetComponent<IMinimapAgent>() == null)
            _player = null;
    }

    private void Start()
    {
        _minimap.RegisterPlayer(_player.GetComponent<IMinimapAgent>());

        foreach (IMinimapAgent agent in _minimapAgents.Select(x => x.GetComponent<IMinimapAgent>()))
            _minimap.Register(agent);
    }
}
