
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BasicPour : UdonSharpBehaviour {

    public GameObject spoutPoint;
    public GameObject target;

    private Fluids2D fluid;

    void Start() {
        fluid = target.GetComponent<Fluids2D>();
    }


    void Update() {
        Vector3 spoutPosition = spoutPoint.transform.position;

        if (spoutPosition.y - transform.position.y < 0.0f) {
            Vector3 spoutForce = spoutPoint.transform.TransformVector(new Vector3(0.0f, 10f, 0.0f));
            fluid.Splat(spoutPosition, spoutForce, new Color(255, 255, 255, 0));
        }
    }
}
