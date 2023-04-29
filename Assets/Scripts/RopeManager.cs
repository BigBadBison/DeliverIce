using System;
using System.Collections.Generic;
using UnityEngine;

public class RopeManager: MonoBehaviour
{
    public static RopeManager Instance;

    [SerializeField] Rope ropePrefab;

    List<Rope> ropes;

    void Awake()
    {
        Instance = this;
        ropes = new List<Rope>();
    }

    public Rope CreateRope()
    {
        Rope rope = Instantiate(ropePrefab, transform);
        ropes.Add(rope);
        return rope;
    }

    public void UpdateRopeAnchors()
    {
        LayerMask walkableLayerMask = LayerMask.GetMask("Walkable");
        foreach (var rope in ropes)
        {
            Collider2D colliderOver = Physics2D.OverlapPoint(rope.start.position, walkableLayerMask);
            IcebergChunk chunk = colliderOver.GetComponentInParent<IcebergChunk>();
            
            rope.AttachStart(chunk.gameObject, rope.start.position);
            if(rope.held) continue;
            colliderOver = Physics2D.OverlapPoint(rope.end.position, walkableLayerMask);
            chunk = colliderOver.GetComponentInParent<IcebergChunk>();
            rope.AttachEnd(chunk.gameObject, rope.end.position);
        }
    }
}
