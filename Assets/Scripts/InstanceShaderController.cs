using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceShaderController : MonoBehaviour
{
    public int MeshIndex;
    private MeshRenderer _meshRenderer;

    void Start()
    {

    }

    public void Init(int meshIndex, int totalMeshes)
    {
        MeshIndex = meshIndex;
        _meshRenderer = GetComponent<MeshRenderer>();
        Material mat = new Material(_meshRenderer.material);
        mat.SetInt("_MeshIndex", MeshIndex);
        mat.SetInt("_TotalMeshes", totalMeshes);
        _meshRenderer.material = mat;
    }

    public void SetTimeScale(float scale)
    {
        _meshRenderer.material.SetFloat("_TimeScale", scale);
    }
}
