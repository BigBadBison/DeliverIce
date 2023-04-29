using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Rope: MonoBehaviour
{
    public DistanceJoint2D distanceJoint = default;
    
    public FixedJoint2D startJoint;
    public FixedJoint2D endJoint;
    
    public Transform start;
    public Transform end;

    LineRenderer lr;

    float maxDistance = 5f;
    
    public Rigidbody2D startRB = default;
    public Rigidbody2D endRB = default;

    public bool held = true;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        startRB = default;
        endRB = default;
    }

    public void AttachStart(GameObject go, Vector3 worldPos)
    {

        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        if (startRB != null)
        {
            if (rb == startRB)
            {
                return;
            }
            Destroy(distanceJoint);
        }
        startRB = rb;
        start.position = worldPos;
        startJoint.connectedBody = startRB;
        distanceJoint = go.AddComponent<DistanceJoint2D>();
        distanceJoint.enableCollision = true;
        distanceJoint.maxDistanceOnly = true;
        distanceJoint.autoConfigureDistance = false;
        distanceJoint.distance = maxDistance;
        distanceJoint.anchor = startRB.GetPoint(worldPos);
    }
    
    public void AttachEnd(GameObject go, Vector3 worldPos)
    {
        Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
        end.position = worldPos;
        endJoint.connectedBody = rb;
        distanceJoint.connectedBody = rb;
        distanceJoint.connectedAnchor = rb.GetPoint(worldPos);
    }
    
    public void SetMaxDistance(float distance)
    {
        maxDistance = distance;
        if (distanceJoint != null)
        {
            distanceJoint.distance = distance;
        }
    }
    
    public void AddSlack(float slack)
    {
        maxDistance = Vector3.Distance(start.position, end.position) + slack;
        distanceJoint.distance = maxDistance;
    }

    void Update()
    {
        Vector3[] positions = {start.position, end.position};
        lr.SetPositions(positions);
    }
}
