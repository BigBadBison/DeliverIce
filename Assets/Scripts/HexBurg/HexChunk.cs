using System;
using System.Collections.Generic;
using UnityEngine;

public class HexChunk : MonoBehaviour {
    public static Vector2 InitialVelocity = Vector2.up * 0f;
    
    [SerializeField] ChunkMesh chunkMesh;
    [SerializeField] HexChunkCollider chunkCollider;
    HexChunk[] neighbors;

    public HexCoordinates coordinates;
    public Rigidbody2D rb;

    void Awake() {
        neighbors = new HexChunk[6];
    }

    public void Triangulate() {
        chunkMesh.Triangulate();
    }

    public void SetNeighbor(HexChunk chunk, HexDirection dir) {
        neighbors[(int)dir] = chunk;
        chunkMesh.hasNeighbor[(int)dir] = true;
    }
}