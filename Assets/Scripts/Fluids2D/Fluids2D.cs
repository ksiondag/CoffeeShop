
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
    private Config config;

    public float splat_force = 0.5f;

    // Materials used for applying shaders (not sure how to apply shader without a material)
    public Material splatMaterial;
    public Material curlMaterial;
    public Material vorticityMaterial;
    public Material divergenceMaterial;
    public Material clearMaterial;

    void Start() {
        dye = GetComponent<Dye>();
        dye.Initialize(512, 512, RenderTextureFormat.ARGB32);

        velocity = GetComponent<Velocity>();
        velocity.Initialize(512, 512, RenderTextureFormat.RGFloat);

        divergence = GetComponent<Divergence>();
        divergence.Initialize(512, 512, RenderTextureFormat.RFloat);

        curl = GetComponent<Curl>();
        curl.Initialize(512, 512, RenderTextureFormat.RFloat);

        pressure = GetComponent<Pressure>();
        pressure.Initialize(512, 512, RenderTextureFormat.RFloat);

        pourer = GetComponent<Pourer>();
        config = GetComponent<Config>();
    }

    void Update() {
        float dt = Time.deltaTime;
        ApplyInputs();
        if (!config.PAUSED) {
            Step(dt);
        }
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

    
    void ApplyInputs() {
        if (pourer.moved) {
            pourer.moved = false;
            SplatPourer();
        }
    }

    void Step(float dt) {
        curlMaterial.SetVector("_TexelSize", velocity.GetTexelSize());
        curlMaterial.SetTexture("_MainTex", velocity.GetTexture());
        curl.Blit(curlMaterial);

        vorticityMaterial.SetVector("_TexelSize", velocity.GetTexelSize());
        vorticityMaterial.SetTexture("_VelocityTex", velocity.GetTexture());
        vorticityMaterial.SetTexture("_CurlTex", curl.GetTexture());
        vorticityMaterial.SetInt("_Curl", config.CURL);
        vorticityMaterial.SetFloat("_DeltaTime", dt);
        velocity.Blit(vorticityMaterial);
        velocity.Swap();

        divergenceMaterial.SetVector("_TexelSize", velocity.GetTexelSize());
        divergenceMaterial.SetTexture("_VelocityTex", velocity.GetTexture());
        divergence.Blit(divergenceMaterial);

        clearMaterial.SetTexture("_MainTex", pressure.GetTexture());
        clearMaterial.SetFloat("_Value", config.PRESSURE);
        pressure.Blit(clearMaterial);
        pressure.Swap();

        // pressureProgram.bind();
        // gl.uniform2f(pressureProgram.uniforms.texelSize, velocity.texelSizeX, velocity.texelSizeY);
        // gl.uniform1i(pressureProgram.uniforms.uDivergence, divergence.attach(0));
        // for (let i = 0; i < config.PRESSURE_ITERATIONS; i++) {
        //     gl.uniform1i(pressureProgram.uniforms.uPressure, pressure.read.attach(1));
        //     blit(pressure.write);
        //     pressure.swap();
        // }

        // gradienSubtractProgram.bind();
        // gl.uniform2f(gradienSubtractProgram.uniforms.texelSize, velocity.texelSizeX, velocity.texelSizeY);
        // gl.uniform1i(gradienSubtractProgram.uniforms.uPressure, pressure.read.attach(0));
        // gl.uniform1i(gradienSubtractProgram.uniforms.uVelocity, velocity.read.attach(1));
        // blit(velocity.write);
        // velocity.swap();

        // advectionProgram.bind();
        // gl.uniform2f(advectionProgram.uniforms.texelSize, velocity.texelSizeX, velocity.texelSizeY);
        // if (!ext.supportLinearFiltering)
        //     gl.uniform2f(advectionProgram.uniforms.dyeTexelSize, velocity.texelSizeX, velocity.texelSizeY);
        // let velocityId = velocity.read.attach(0);
        // gl.uniform1i(advectionProgram.uniforms.uVelocity, velocityId);
        // gl.uniform1i(advectionProgram.uniforms.uSource, velocityId);
        // gl.uniform1f(advectionProgram.uniforms.dt, dt);
        // gl.uniform1f(advectionProgram.uniforms.dissipation, config.VELOCITY_DISSIPATION);
        // blit(velocity.write);
        // velocity.swap();

        // if (!ext.supportLinearFiltering)
        //     gl.uniform2f(advectionProgram.uniforms.dyeTexelSize, dye.texelSizeX, dye.texelSizeY);
        // gl.uniform1i(advectionProgram.uniforms.uVelocity, velocity.read.attach(0));
        // gl.uniform1i(advectionProgram.uniforms.uSource, dye.read.attach(1));
        // gl.uniform1f(advectionProgram.uniforms.dissipation, config.DENSITY_DISSIPATION);
        // blit(dye.write);
        // dye.swap();
    }

    void Render() {

    }

    void SplatPourer() {
        Vector2 delta = pourer.delta * splat_force;
        Splat(pourer.texcoord, delta, pourer.color);
    }

    void Splat(Vector2 position, Vector2 force, Color color) {
        splatMaterial.SetTexture("_MainTex", velocity.GetTexture());
        // TODO: I don't think aspect ratio should ever be non-1, but maybe handle it anyways?
        splatMaterial.SetFloat("_AspectRatio", 1.0f);
        splatMaterial.SetVector("_Point", new Vector4(position.x, position.y, 0, 0));
        splatMaterial.SetVector("_Color", new Vector4(force.x, force.y, 0, 0));
        splatMaterial.SetFloat("_Radius", config.SPLAT_RADIUS);
        velocity.Blit(splatMaterial);
        velocity.Swap();

        splatMaterial.SetTexture("_MainTex", dye.GetTexture());
        splatMaterial.SetVector("_Color", new Vector4(color.r, color.g, color.b, 0));
        dye.Blit(splatMaterial);
        dye.Swap();
    }
}
