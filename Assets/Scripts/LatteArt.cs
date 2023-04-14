using System.Collections;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LatteArt : UdonSharpBehaviour
{
    public Material fillMaterial;

    [Range(0, 1)] public float fillAmount = 0.5f;
    public float fillRate = 10.0f;
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
