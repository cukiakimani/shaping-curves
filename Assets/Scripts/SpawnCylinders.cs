using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCylinders : MonoBehaviour
{
    public GameObject Cylinder;
    public int TotalMeshes;

    [Range(1f, 25f)]
    public float TimeScale;

    private List<InstanceShaderController> _shaderControllers;

    void Start()
    {
        _shaderControllers = new List<InstanceShaderController>();

        for (int i = 0; i < TotalMeshes; i++)
        {
            GameObject mesh = Instantiate(Cylinder, transform.position, Quaternion.identity);
            InstanceShaderController shaderController = mesh.GetComponent<InstanceShaderController>();
            shaderController.Init(i, TotalMeshes);
            _shaderControllers.Add(shaderController);
            mesh.transform.parent = this.transform;
        }
    }

    void Update()
    {
        foreach (var s in _shaderControllers)
        {
            s.SetTimeScale(TimeScale);
        }
    }
}
