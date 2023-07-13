
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RotationControl : UdonSharpBehaviour {
    public float position = 0.0f;
    public float height = 0.0f;
    private Vector3 startPosition;
    private Vector3 startSpoutPosition;
    private float startMinHeight;

    public float rotation = 0.0f;
    private Quaternion startRotation;

    public GameObject spoutPoint;
    public GameObject distanceHelper;
    public GameObject target;

    private MeshFilter meshFilter;

    private Vector3 getSpoutPoint() {
        return spoutPoint.transform.TransformPoint(spoutPoint.GetComponent<MeshFilter>().mesh.vertices[0]);
    }

    void Start() {
        startPosition = transform.position;
        startSpoutPosition = getSpoutPoint();
        startRotation = transform.rotation;

        meshFilter = distanceHelper.GetComponent<MeshFilter>();

        startMinHeight = CurrentHeight();
    }

    private float CurrentHeight() {
        Vector3[] vertices = meshFilter.mesh.vertices;

        Bounds bounds = GetComponent<Renderer>().bounds;
        float worldMinY = bounds.max.y;
        Bounds targetBounds = target.GetComponent<Renderer>().bounds;

        foreach (Vector3 vertex in vertices)
        {
            // Convert local coordinates to world coordinates
            Vector3 worldVertex = distanceHelper.transform.TransformPoint(vertex);

            if (worldVertex.x > targetBounds.min.x && worldVertex.x < targetBounds.max.x && worldVertex.z > targetBounds.min.z && worldVertex.z < targetBounds.max.z) {
                // Update the minimum and maximum coordinates
                worldMinY = Mathf.Min(worldMinY, worldVertex.y);
            }

        }

        return worldMinY;
    }

    void Update() {

        transform.rotation = startRotation;
        transform.position = startPosition;

        transform.rotation = startRotation * Quaternion.Euler(0, rotation, 0);

        float deltaX = position - (getSpoutPoint().x - startSpoutPosition.x);
        transform.position = startPosition + new Vector3(deltaX, 0, 0);
        float minAboveTarget = CurrentHeight();
        transform.position = startPosition + new Vector3(deltaX, startMinHeight - minAboveTarget + height, 0);
        // Debug.Log(1000*(getSpoutPoint() - startSpoutPosition));
    }
}
