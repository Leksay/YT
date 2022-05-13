using UnityEngine;

[ExecuteInEditMode]
public class PlayerShaderPropertiesSetter : MonoBehaviour
{
    //SP = Shader property
    private static int SP_PlayerWorldPosition = Shader.PropertyToID("_PlayerWorldPos");
    private Transform _transform;

    private void Start()
    {
        _transform = transform;
    }

    private void Update()
    {
        Shader.SetGlobalVector(SP_PlayerWorldPosition, _transform.position);
    }
}
