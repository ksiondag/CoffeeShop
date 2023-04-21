using System.Collections;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LatteArt : UdonSharpBehaviour
{
    public Material fillMaterial;

    [Range(0, 1)] public float fillAmount = 0.0f;
    public float fillRate = 0.001f;
    public float maxFill = 1.0f;

    private bool isColliding = false;

    private void OnCollisionEnter(Collision collision)
    {
        EspressoMachine espressoMachine = collision.gameObject.GetComponent<EspressoMachine>();
        if (espressoMachine != null)
        {
            // Set isColliding to true when colliding with "OtherObject"
            isColliding = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        EspressoMachine espressoMachine = collision.gameObject.GetComponent<EspressoMachine>();
        if (espressoMachine != null)
        {
            // Set isColliding to false when no longer colliding with "OtherObject"
            isColliding = false;
        }
    }

    private void IncreaseFill()
    {
        fillAmount += fillRate * Time.deltaTime;
        fillAmount = Mathf.Clamp(fillAmount, 0.0f, maxFill);
        Debug.Log("Current Fill Value: " + fillAmount);
    }

    void Start()
    {
        UpdateShaderParameters();
    }

    void Update()
    {
        UpdateShaderParameters();
        if (isColliding)
        {
            IncreaseFill();
        }
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
