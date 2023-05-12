using System.Collections;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LatteArt : UdonSharpBehaviour
{
    public Material fillMaterial;

    [Range(0, 1)] public float fillAmount = 0.0f;
    public float maxFill = 1.0f;
    public Color color = Color.blue;

    void Start()
    {
        UpdateShaderParameters();
    }

    void Update()
    {
        UpdateShaderParameters();
    }

    public void FillByAmount(float addAmount, Color addColor)
    {
        // Calculate the new fill amount
        float totalFillAmount = Mathf.Clamp(fillAmount + addAmount, 0.0f, maxFill);

        // Calculate the weight of the new color in the mix
        float addColorWeight = addAmount / totalFillAmount;

        // Blend the current fill color with the new color
        Color mixedColor = Color.Lerp(color, addColor, addColorWeight);

        // Update the fill color and amount in the material
        fillMaterial.SetColor("_FillColor", mixedColor);
        fillMaterial.SetFloat("_FillAmount", totalFillAmount);

        // Update the current fill color and amount for the next frame
        color = mixedColor;
        fillAmount = totalFillAmount;
    }

    private void UpdateShaderParameters()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        if (meshFilter.mesh == null)
        {
            Debug.LogError("MeshFilter.sharedMesh is null");
            return;
        }

        Vector3[] vertices = meshFilter.mesh.vertices;

        Vector3 worldMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 worldMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        foreach (Vector3 vertex in vertices)
        {
            // Convert local coordinates to world coordinates
            Vector3 worldVertex = transform.TransformPoint(vertex);

            // Update the minimum and maximum coordinates
            worldMin = Vector3.Min(worldMin, worldVertex);
            worldMax = Vector3.Max(worldMax, worldVertex);
        }

        Debug.Log("World Min: " + worldMin);
        Debug.Log("World Max: " + worldMax);
        
        fillMaterial.SetVector("_WorldMin", worldMin);
        fillMaterial.SetVector("_WorldMax", worldMax);
        fillMaterial.SetFloat("_FillAmount", fillAmount);
        meshRenderer.material = fillMaterial;
    }
}
