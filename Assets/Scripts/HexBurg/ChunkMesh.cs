using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ChunkMesh : MonoBehaviour {
    Mesh chunkMesh;
    List<Vector3> vertices;
    List<int> triangles;

    Vector3 height = new Vector3(0f, 0f, -1f);

    public float breakFraction = 0.95f;

    public bool[] hasNeighbor;
    
    void Awake() {
        GetComponent<MeshFilter>().mesh = chunkMesh = new Mesh();
        hasNeighbor = new[] {false, false, false, false, false, false};
        chunkMesh.name = "Chunk Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
    }

    void Start() {
        Triangulate();
    }

    public void Triangulate() {
        chunkMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        Vector3 centerTop = transform.localPosition;
        Vector3 centerBottom = centerTop - height;
        for (HexDirection dir = HexDirection.NE; dir <= HexDirection.NW; dir++) {
            Vector3 bridge = HexMetrics.GetBridge(dir);
            Vector3 v1t = centerTop + HexMetrics.GetFirstSolidCorner(dir);
            Vector3 v2t = centerTop + HexMetrics.GetSecondSolidCorner(dir);
            Vector3 v3t = v1t + bridge;
            Vector3 v4t = v2t + bridge;

            if (!hasNeighbor[(int)dir]) {
                v3t -= height * HexMetrics.blendFactor;
                v4t -= height * HexMetrics.blendFactor;
            }
            
            Vector3 vc1t = centerTop + HexMetrics.GetFirstCorner(dir);
            Vector3 vc2t = centerTop + HexMetrics.GetSecondCorner(dir);
            
            if (!hasNeighbor[(int)dir]) {
                vc1t -= height * HexMetrics.blendFactor * 0.5f;
                vc2t -= height * HexMetrics.blendFactor * 0.5f;
            }
            if (!hasNeighbor[(int)dir.Previous()]) {
                vc1t -= height * HexMetrics.blendFactor * 0.5f;
            }
            if (!hasNeighbor[(int)dir.Next()]) {
                vc2t -= height * HexMetrics.blendFactor * 0.5f;
            }
            
            AddTriangle(centerTop, v1t, v2t);
            AddQuad(v1t, v2t, v3t, v4t);
            AddTriangle(v1t, vc1t, v3t);
            AddTriangle(v2t, v4t, vc2t);
            
            Vector3 v1b = centerBottom + HexMetrics.GetFirstSolidCorner(dir);
            Vector3 v2b = centerBottom + HexMetrics.GetSecondSolidCorner(dir);
            Vector3 v3b = v1b + bridge;
            Vector3 v4b = v2b + bridge;
            Vector3 vc1b = centerBottom + HexMetrics.GetFirstCorner(dir);
            Vector3 vc2b = centerBottom + HexMetrics.GetSecondCorner(dir);
            
            AddTriangle(centerBottom, v2b, v1b);
            AddQuad(v1b, v3b, v2b, v4b);
            AddTriangle(v1b, v3b, vc1b);
            AddTriangle(v2b, vc2b, v4b);
            
            AddQuad(vc1t, v3t, vc1b, v3b);
            AddQuad(v3t, v4t, v3b, v4b);
            AddQuad(v4t, vc2t, v4b, vc2b);
        }
        chunkMesh.vertices = vertices.ToArray();
        chunkMesh.triangles = triangles.ToArray();
        chunkMesh.RecalculateNormals();
    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }
    void AddQuad (Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }
}