
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PaintWithMouse : UdonSharpBehaviour {
    private Material material;

    public Material shaderMaterial;
    public MeshRenderer targetRenderer;

    private RenderTexture renderTexture;

    void Start() {
        material = GetComponent<Renderer>().material;
        renderTexture = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGB32);
        targetRenderer.material.mainTexture = renderTexture;
    }

    private void UpdateFromCollision(Collision collision) {
        Vector3 point = collision.GetContact(0).point;
        Vector3 localPoint = this.gameObject.transform.InverseTransformPoint(point);
        Debug.Log("Local point: " + localPoint);

        // Convert local coordinates to UV coordinates
        Vector2 uvPoint;
        uvPoint.x = 1 - (localPoint.x + 5f) / 10f;
        uvPoint.y = 1 - (localPoint.z + 5f) / 10f;

        material.SetVector("_InteractionPoint", new Vector4(uvPoint.x, uvPoint.y, 0, 0));
        shaderMaterial.SetVector("_InteractionPoint", new Vector4(uvPoint.x, uvPoint.y, 0, 0));
        VRCGraphics.Blit(null, renderTexture, shaderMaterial);
    }

    private void OnCollisionEnter(Collision collision) {
        UpdateFromCollision(collision);
    }

    private void OnCollisionStay(Collision collision) {
        UpdateFromCollision(collision);
    }
}
