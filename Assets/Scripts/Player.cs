using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    [SerializeField] float walkingForce = 2f;
    [SerializeField] float jumpTime = 1f;
    [SerializeField] float minRopeLength = 1f;
    [SerializeField] float maxRopeLength = 5f;
    [SerializeField] float ropeSlack = 2f;
    [SerializeField] float chunkFriction = 5f;

    HexChunk activeChunk;

    CircleCollider2D collider;
    Rigidbody2D rb;
    LayerMask walkableLayerMask;

    Rope rope;

    bool wantJump = false;
    bool jumping = false;
    bool holdingRope = false;

    void Awake() {
        walkableLayerMask = LayerMask.GetMask("Walkable");
        collider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start() {
        rb.velocity = HexChunk.InitialVelocity;
    }

    void Update() {
        wantJump |= Input.GetButtonDown("Jump");

        if (Input.GetButtonDown("Fire1")) {
            Collider2D colliderOver = Physics2D.OverlapPoint(transform.position, walkableLayerMask);
            activeChunk = colliderOver.GetComponentInParent<HexChunk>();
            if (activeChunk == null) return;
            if (!holdingRope) {
                rope = RopeManager.Instance.CreateRope();
                rope.SetMaxDistance(maxRopeLength);
                rope.AttachStart(activeChunk.gameObject, transform.position);
                rope.AttachEnd(gameObject, transform.position);
            }
            else {
                rope.AttachEnd(activeChunk.gameObject, transform.position);
                rope.AddSlack(ropeSlack);
                rope.held = false;
                rope = null;
            }

            holdingRope = !holdingRope;
        }
    }

    void FixedUpdate() {
        Collider2D colliderOver = Physics2D.OverlapPoint(transform.position, walkableLayerMask);
        if (!collider.IsTouchingLayers(walkableLayerMask)) {
            Debug.Log("GAME OVER");
        }


        if (colliderOver != null) {
            activeChunk = colliderOver.GetComponentInParent<HexChunk>();
            Vector2 force = GetMovementForce();
            force += GetFrictionForce();

            rb.AddForce(force);
            activeChunk.rb.AddForce(-force);
        }

        ;

        if (wantJump) {
            wantJump = false;
            // Jump();
        }
    }

    Vector2 GetMovementForce() {
        Vector2 force = Vector2.up * Input.GetAxis("Vertical") * walkingForce;
        force += Vector2.right * Input.GetAxis("Horizontal") * walkingForce;
        return force;
    }

    Vector2 GetFrictionForce() {
        Vector3 deltaV = activeChunk.rb.velocity - rb.velocity;
        return deltaV * chunkFriction;
    }

    void Jump() {
        if (jumping) return;
        StartCoroutine(JumpRoutine(this));
    }

    IEnumerator JumpRoutine(Player player) {
        print("JUMP");
        player.jumping = true;
        Transform xform = player.transform;
        Vector3 pos = xform.localPosition;
        pos.z -= 2.5f;
        xform.localPosition = pos;
        yield return new WaitForSeconds(player.jumpTime);
        pos = xform.localPosition;
        pos.z += 2.5f;
        xform.localPosition = pos;
        player.jumping = false;
        print("END JUMP");
    }
}