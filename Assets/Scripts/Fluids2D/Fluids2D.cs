
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Fluids2D : UdonSharpBehaviour {
    private Dye dye;
    private Velocity velocity;
    private Divergence divergence;
    private Curl curl;
    private Pressure pressure;

    private Pourer pourer;

    public float splat_force = 0.5f;

    public Material splatMaterial;

    void Start() {
        dye = GetComponent<Dye>();
        dye.Initialize(512, 512, RenderTextureFormat.ARGB32);

        velocity = GetComponent<Velocity>();
        velocity.Initialize(512, 512, RenderTextureFormat.RGFloat);

        // divergence = GetComponent<Divergence>();

        // curl = GetComponent<Curl>();

        // pressure = GetComponent<Pressure>();

        pourer = GetComponent<Pourer>();
    }

    void Update() {
        float dt = Time.deltaTime;
        ApplyInputs();
    }

    private Vector2 ConvertToUV(Vector3 point) {
        Vector3 localPoint = this.gameObject.transform.InverseTransformPoint(point);

        // Convert local coordinates to UV coordinates
        Vector2 uvPoint;
        uvPoint.x = 1 - (localPoint.x + 5f) / 10f;
        uvPoint.y = 1 - (localPoint.z + 5f) / 10f;
        return uvPoint;
    }

    private void OnCollisionEnter(Collision collision) {
        Vector3 point = collision.GetContact(0).point;
        Vector2 uvPoint = ConvertToUV(point);
        pourer.Initialize(uvPoint);
    }

    private void OnCollisionStay(Collision collision) {
        Vector3 point = collision.GetContact(0).point;
        Vector2 uvPoint = ConvertToUV(point);
        pourer.Move(uvPoint);
    }

    private void OnCollisionExit(Collision collision) {
        pourer.Reset();
    }

    void Step(float dt) {

    }

    void Render() {

    }

    void ApplyInputs() {
        if (pourer.moved) {
            pourer.moved = false;
            SplatPourer();
        }
    }

    void SplatPourer() {
        Vector2 delta = pourer.delta * splat_force;
        Splat(pourer.texcoord, delta, pourer.color);
    }

    void Splat(Vector2 position, Vector2 force, Color color) {
        // gl.uniform1i(splatProgram.uniforms.uTarget, velocity.read.attach(0));
        splatMaterial.SetTexture("_MainTex", velocity.GetTexture());
        splatMaterial.SetFloat("_AspectRatio", 1.0f);
        splatMaterial.SetVector("_Point", new Vector4(position.x, position.y, 0, 0));
        splatMaterial.SetVector("_Color", new Vector4(force.x, force.y, 0, 0));
        splatMaterial.SetFloat("_Radius", 0.01f);
        velocity.Blit(splatMaterial);
        velocity.Swap();

        splatMaterial.SetTexture("_MainTex", dye.GetTexture());
        splatMaterial.SetVector("_Color", new Vector4(color.r, color.g, color.b, 0));
        dye.Blit(splatMaterial);
        dye.Swap();
    }
}
