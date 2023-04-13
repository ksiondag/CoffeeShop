using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[ExecuteInEditMode]
public class LatteArt : UdonSharpBehaviour
{
    public Material fillMaterial;
    [Range(0, 1)] public float fillAmount = 0.5f;

    void Start()
    {
        UpdateShaderParameters();
    }

    void Update()
    {
        UpdateShaderParameters();
    }

    private void UpdateShaderParameters()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Bounds localBounds = meshFilter.mesh.bounds;

        Vector3 worldMin = transform.TransformPoint(localBounds.min);
        Vector3 worldMax = transform.TransformPoint(localBounds.max);
        
        fillMaterial.SetFloat("_LocalMin.x", worldMin.x);
        fillMaterial.SetFloat("_LocalMin.y", worldMin.y);
        fillMaterial.SetFloat("_LocalMin.z", worldMin.z);
        fillMaterial.SetFloat("_LocalMin.w", 1);
        fillMaterial.SetFloat("_LocalMax.x", worldMax.x);
        fillMaterial.SetFloat("_LocalMax.y", worldMax.y);
        fillMaterial.SetFloat("_LocalMax.z", worldMax.z);
        fillMaterial.SetFloat("_LocalMax.w", 1);
        fillMaterial.SetFloat("_FillAmount", fillAmount);
        meshRenderer.material = fillMaterial;
    }
}
