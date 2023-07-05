
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PaintWithMouse : UdonSharpBehaviour {
    private Material material;

    public Material shaderMaterial;

    private Dye dye;

    void Start() {
        material = GetComponent<Renderer>().material;
        dye = GetComponent<Dye>();
        dye.Initialize(512, 512, RenderTextureFormat.ARGB32);
    }

    private void UpdateFromCollision(Collision collision) {
        Vector3 point = collision.GetContact(0).point;
        Vector3 localPoint = this.gameObject.transform.InverseTransformPoint(point);

        // Convert local coordinates to UV coordinates
        Vector2 uvPoint;
        uvPoint.x = 1 - (localPoint.x + 5f) / 10f;
        uvPoint.y = 1 - (localPoint.z + 5f) / 10f;

        material.SetVector("_InteractionPoint", new Vector4(uvPoint.x, uvPoint.y, 0, 0));
        shaderMaterial.SetVector("_InteractionPoint", new Vector4(uvPoint.x, uvPoint.y, 0, 0));
        dye.Blit(shaderMaterial);
    }

    private void OnCollisionEnter(Collision collision) {
        UpdateFromCollision(collision);
    }

    private void OnCollisionStay(Collision collision) {
        UpdateFromCollision(collision);
    }
}
