using UnityEngine;

public class SuperSimpleFollower : MonoBehaviour, IMinimapAgent
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

    #region fields

    [SerializeField] private float _speed;
    [SerializeField] private Transform _followTarget;
    
    private CharacterController _characterController;

    #endregion

    #region engine methods

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector3 move = ((_followTarget.position - transform.position).normalized * _speed + Physics.gravity) * Time.deltaTime;

        _characterController.Move(move);
    }

    #endregion
}
