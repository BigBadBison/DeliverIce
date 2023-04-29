using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexBurg : MonoBehaviour
{
    public int radius = 6;

    public HexChunk chunkPrefab;

    Hashtable tileMap;
    public HexChunk[] chunks;

    void Awake()
    {
        int tileCount = 3 * radius * radius + 3 * radius + 1;
        chunks = new HexChunk[tileCount];
        tileMap = new Hashtable();
        for (int q = -radius, i = 0; q <= radius; q++) {
            int minR = Math.Max(-radius, -q - radius);
            int maxR = Math.Min(radius, -q + radius);
            for (int r = minR; r <= maxR; r++) {
                CreateCell(q, r, i++);
            }
        }
        SetNeighbors();
        Triangulate();
    }
    
    void CreateCell (int q, int r, int i) {
        Vector3 position = new Vector3();
        position += HexMetrics.qVector * q;
        position += HexMetrics.rVector * r;

        HexChunk chunk = chunks[i] = Instantiate(chunkPrefab);
        chunk.coordinates = new HexCoordinates(q, r);
        tileMap.Add(chunk.coordinates.GetHashCode(), chunk);

        chunk.transform.SetParent(transform, false);
        chunk.transform.localPosition = position;
    }

    void SetNeighbors() {
        for (int i = 0; i < chunks.Length; i++) {
            SetNeighbors(i);
        }
    }
    
    void SetNeighbors(int idx) {
        var chunk = chunks[idx];
        for (int i = 0; i < HexMetrics.neighborCoordinates.Length; i++) {
            var dir = HexMetrics.neighborCoordinates[i];
            HexCoordinates coords = new HexCoordinates(chunk.coordinates.Q + dir.Q, chunk.coordinates.R + dir.R);
            HexChunk neighbor = GetTile(coords);
            Debug.Log(neighbor);
            if (neighbor != null) {
                chunk.SetNeighbor(neighbor, (HexDirection)i);
            }
        }
    }

    void Triangulate() {
        for (int i = 0; i < chunks.Length; i++) {
            var chunk = chunks[i];
            chunk.Triangulate();
        }
    }
    
    public HexChunk GetTile (HexCoordinates coords) {
        return (HexChunk)tileMap[coords.GetHashCode()];
    }
}
