using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public Vector3 offset;
    public float smoothing;
    
    // Start is called before the first frame update
    void Start()
    {
        transform.position = target.position - offset;
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position - offset, smoothing);
    }
}
