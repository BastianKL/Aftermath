using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.HighDefinition;

public class Floater : MonoBehaviour
{
    public Rigidbody rigidBody;
    public float depthBeforeSubmerged;
    public float displacementAmount;
    public int floatersCount;

    public float waterDrag;
    public float WaterAngularDrag;
    public WaterSurface water;

    WaterSearchParameters Search;
    WaterSearchResult SearchResult;

    private void FixedUpdate()
    {
        rigidBody.AddForceAtPosition(Physics.gravity / floatersCount, transform.position, ForceMode.Acceleration);
        Search.startPositionWS = transform.position;
        water.ProjectPointOnWaterSurface(Search, out SearchResult);
        if (transform.position.y < SearchResult.projectedPositionWS.y)
        {
            float displacementMulti = Mathf.Clamp01((SearchResult.projectedPositionWS.y - transform.position.y) / depthBeforeSubmerged) * displacementAmount;
            rigidBody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMulti, 0f), transform.position, ForceMode.Acceleration);
            rigidBody.AddForce(displacementMulti * -rigidBody.linearVelocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
            rigidBody.AddTorque(displacementMulti * -rigidBody.angularVelocity * WaterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}
