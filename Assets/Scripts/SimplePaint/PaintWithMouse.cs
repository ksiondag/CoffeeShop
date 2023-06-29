
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PaintWithMouse : UdonSharpBehaviour {
    private Material material;

    void Start() {
        material = GetComponent<Renderer>().material;
    }

    private void OnCollisionEnter(Collision collision) {
        Vector3 point = collision.GetContact(0).point;
        Vector3 localPoint = this.gameObject.transform.InverseTransformPoint(point);
        material.SetVector("_InteractionPoint", new Vector4(localPoint.x, localPoint.y, localPoint.z, 0));
    }
}
