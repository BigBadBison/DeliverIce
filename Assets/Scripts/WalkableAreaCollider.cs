using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkableAreaCollider : MonoBehaviour
{
    PolygonCollider2D collider;

    void Awake() {
        collider = GetComponent<PolygonCollider2D>();
        List<Vector2> points = new List<Vector2>();
        Vector3 center = transform.localPosition;
        for (int i = 0; i < 6; i++) {
            Vector3 pos = center + HexMetrics.corners[i] * 1.25f;
            points.Add(new Vector2(pos.x, pos.y));
        }

        collider.points = points.ToArray();
    }
}
