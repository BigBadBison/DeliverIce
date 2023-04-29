using System;
using System.Collections.Generic;
using System.Linq;
using TreeEditor;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class IcebergChunk : MonoBehaviour {
    public List<Vector2> vertices2D;

    [SerializeField] IcebergChunk chunkPrefab;
    [SerializeField] int numPoints = 20;
    [SerializeField] float height = 1;
    [SerializeField] float radius = 7.5f;
    [SerializeField] float irregularity = 2f;
    [SerializeField] int seed = 1;

    [SerializeField] float minMass = 20f;
    
    [SerializeField] float maxCrackLength = 1f;
    [SerializeField] float minCrackLength = 0.25f;
    [SerializeField] float maxCrackAngle = 1f;
    
    public static Vector2 InitialVelocity = Vector2.up * 2f;

    Mesh msh;
    PolygonCollider2D chunkCollider;
    PolygonCollider2D walkableCollider;
    MeshFilter mf;
    public Rigidbody2D rb;
    
    void Awake() {
        foreach (var joint in GetComponents<DistanceJoint2D>())
        {
            Destroy(joint);
        }

        mf = GetComponent<MeshFilter>();
        mf.mesh = msh = new Mesh();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = InitialVelocity;
        chunkCollider = GetComponent<PolygonCollider2D>();
        walkableCollider = GetComponentsInChildren<PolygonCollider2D>()[1];
        msh.name = "Surface Mesh";
        InitializeVertices();
    }

    void Start () {
        UpdateMesh();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha0)) {
            Damage(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Damage(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            Damage(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) {
            Damage(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) {
            Damage(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5)) {
            Damage(5);
        }
    }

    void FixedUpdate()
    {
        rb.AddForce(WaterCurrent.GetWaterCurrentForce(rb.velocity));
    }

    void InitializeVertices() {
        List<Vector2> vertices = new List<Vector2>();
        for (int i = 0; i < numPoints; i++) {
            var angleDeg = 180.0f - 360.0f * i / numPoints;
            var angleRad = Mathf.Deg2Rad * angleDeg;
            vertices.Add(new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * (radius + Random.Range(-irregularity, irregularity)));
            // print(new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * radius);
        }
        vertices2D = vertices;
    }

    void UpdateMesh() {
        Destroy(msh);
        if (vertices2D.Count < 3)
        {
            Destroy(gameObject);
            return;
        }

        var verticesArray = vertices2D.ToArray();

        chunkCollider.points = verticesArray;
        walkableCollider.points = verticesArray;
        
        msh = chunkCollider.CreateMesh(false, false);
        mf.mesh = msh;
        print(chunkCollider.points.Length);
        List<Vector3> vertices = msh.vertices.ToList();
        List<int> triangles = msh.triangles.ToList();
        for (int i=0; i < msh.vertices.Length; i++)
        {
            var v = msh.vertices[i];
            vertices.Add(new Vector3(v.x, v.y, height));
        }
        for (int i=0; i * 3 + 2 < msh.triangles.Length; i++)
        {
            var i1 = triangles[i * 3];
            var i2 = triangles[i * 3 + 1];
            var i3 = triangles[i * 3 + 2];
            
            triangles.Add(i1 + msh.vertices.Length);
            triangles.Add(i3 + msh.vertices.Length);
            triangles.Add(i2 + msh.vertices.Length);
            
            triangles.Add(i1);
            triangles.Add(i1 + msh.vertices.Length);
            triangles.Add(i2);
            
            triangles.Add(i2);
            triangles.Add(i1 + msh.vertices.Length);
            triangles.Add(i2 + msh.vertices.Length);
            
            triangles.Add(i2);
            triangles.Add(i2 + msh.vertices.Length);
            triangles.Add(i3);
            
            triangles.Add(i3);
            triangles.Add(i2 + msh.vertices.Length);
            triangles.Add(i3 + msh.vertices.Length);
            
            triangles.Add(i3);
            triangles.Add(i3 + msh.vertices.Length);
            triangles.Add(i1);
            
            triangles.Add(i1);
            triangles.Add(i3 + msh.vertices.Length);
            triangles.Add(i1 + msh.vertices.Length);
        }

        // msh.vertices = vertices.ToArray();
        // msh.triangles = triangles.ToArray();
        
        msh.RecalculateBounds();
        msh.RecalculateNormals();
        msh.RecalculateTangents();

        RopeManager.Instance.UpdateRopeAnchors();
    }


    void Damage(int inIndex) {
        float loc = Random.Range(0.1f, 0.9f);
        Vector2 hitLeft = vertices2D[inIndex];
        Vector2 hitRight = vertices2D[(inIndex + 1) % vertices2D.Count];
        Vector2 inPoint = Vector2.Lerp(hitLeft, hitRight, loc);
        Vector2 line = hitLeft - hitRight;
        Vector2 normal = new Vector2(-line.y, line.x).normalized;
        Damage(inPoint, normal);
    }

    void Damage(Vector2 inPoint, Vector2 normal) {
        Debug.Log("DAMAGING");
        Vector2[] crackPoints = GenerateCrack(inPoint, normal);
        Vector2 outPoint = crackPoints[crackPoints.Length - 1];
        int inIndex = getPointStartIndex(inPoint);
        int outIndex = getPointStartIndex(outPoint);

        int startIndex = inIndex;
        Vector2 startPoint = inPoint;
        int endIndex = outIndex;
        Vector2 endPoint = outPoint;
        if (outIndex < inIndex)
        {
            startIndex = outIndex;
            startPoint = outPoint;
            endIndex = inIndex;
            endPoint = inPoint;
        }
        
        IcebergChunk newChunk = Instantiate(chunkPrefab, transform.parent);
        newChunk.vertices2D = new List<Vector2>(vertices2D.GetRange(startIndex + 1, endIndex - startIndex));
        vertices2D.RemoveRange(startIndex + 1, endIndex - startIndex);
        vertices2D.InsertRange(startIndex + 1, crackPoints);
        newChunk.vertices2D.AddRange(crackPoints.Reverse());
        newChunk.rb.velocity = rb.velocity;
        UpdateMesh();
    }

    Vector2[] GenerateCrack(Vector2 inPoint, Vector2 normal)
    {
        List<Vector2> points = new List<Vector2>();
        points.Add(inPoint);
        
        Vector2 point = inPoint;
        Vector2 norm = normal;
        
        while (true)
        {
            float crackLength = Random.Range(minCrackLength, maxCrackLength);
            float crackAngle = Random.Range(-maxCrackAngle, maxCrackAngle);
            norm = Quaternion.Euler(0, 0, crackAngle) * norm;
            norm.Normalize();
            Vector2 rayOrigin = transform.TransformPoint(point + crackLength * norm);
            Vector2 rayNormal = transform.TransformDirection(-norm);
            print("Ray Origin");
            print(rayOrigin);
            print("Ray Nomral");
            print(rayNormal);
            RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, rayNormal);
            Vector2 outPoint = default;
            foreach (var hit in hits)
            {
                if (hit.collider != chunkCollider) continue;
                if (hit.distance < 0.01f) continue;
                print("HIT");
                print(hit.distance);
                Vector3 localPoint = transform.InverseTransformPoint(hit.point);
                points.Add(localPoint);
                print(points.Count);
                return points.ToArray();
            }
            point += crackLength * norm;
            print(point);
            points.Add(point);
            if (points.Count > 10)
            {
                print("NO END FOUND");
                return points.ToArray();
            }
        }
    }
    
    int getPointStartIndex(Vector2 point)
    {
        for (int i = 0; i < vertices2D.Count; i++)
        {
            var start = vertices2D[i];
            var end = vertices2D[(i + 1) % vertices2D.Count];
            float dSE = Vector2.Distance(start, end);
            float dSP = Vector2.Distance(start, point);
            float dEP = Vector2.Distance(end, point);
            if (dSE > dSP && dSE > dEP)
            {
                return i;
            }
            // print(Vector2.Angle(start - end, start - point));
            // if (Vector2.Angle(start - end, start - point) < 2.5f)
            // {
            //     return i;
            // }
        }
        print("POINT NOT FOUND");
        print(point);
        return 0;
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer != 9) return;
        if (rb.mass < minMass) return;
        
        ContactPoint2D contactPoint = collision.GetContact(0);
        print("---CONTACT POINT---");
        print(contactPoint.normalImpulse);
        print(contactPoint.tangentImpulse);
        print("-------------------");
        var localPoint = WorldPointToLocal(contactPoint.point);
        var localDir = WorldDirToLocal(contactPoint.normal).normalized;
        Damage(localPoint, localDir);
        if (collision.collider.gameObject.layer == 9)
        {
            collision.collider.gameObject.SetActive(false);
        }
    }

    Vector2 WorldPointToLocal(Vector2 point)
    {
        return transform.InverseTransformPoint(point);
    }
    
    Vector2 WorldDirToLocal(Vector2 direction)
    {
        return transform.InverseTransformDirection(direction);
    }
    
    void OnDrawGizmosSelected()
    {
        if (msh == null)
        {
            return;
        }
        for (int i = 0; i < msh.vertices.Length; i++)
        {
            Gizmos.color = new Color(1.0f * i / msh.vertices.Length, 0, 0);
            var worldPos= transform.TransformPoint(msh.vertices[i]);
            Gizmos.DrawCube(worldPos, Vector3.one * 0.5f);
        }
    }
}