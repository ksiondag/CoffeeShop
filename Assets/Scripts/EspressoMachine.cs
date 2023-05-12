
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class EspressoMachine : UdonSharpBehaviour
{
    // Constraint: Only handles one latte art at a time
    // TODO: Allow multiple latte arts to be made at once
    public float fillRate = 0.01f;
    public Color espressoColor = new Color(30, 19, 17, 255);

    private bool isColliding = false;
    private LatteArt latteArt = null;

    private void OnCollisionEnter(Collision collision)
    {
        latteArt = collision.gameObject.GetComponent<LatteArt>();
        if (latteArt != null)
        {
            // Set isColliding to true when colliding with "OtherObject"
            isColliding = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        LatteArt latteArt = collision.gameObject.GetComponent<LatteArt>();
        if (latteArt != null)
        {
            // Set isColliding to false when no longer colliding with "OtherObject"
            isColliding = false;
        }
    }

    private void IncreaseFill()
    {
        if (latteArt != null)
        {
            latteArt.FillByAmount(fillRate * Time.deltaTime, espressoColor);
            Debug.Log("Current Fill Value: " + latteArt.fillAmount);
        }
    }

    void Update()
    {
        if (isColliding)
        {
            IncreaseFill();
        }
    }

}
