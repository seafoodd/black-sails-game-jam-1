using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform startingPos;
    [SerializeField] private PlayerMovement pm;
    [SerializeField] private LineRenderer rope;
    [SerializeField] private EdgeCollider2D edgeCollider;
    [SerializeField] private LayerMask collMask;
    [SerializeField] private float minCollisionDistance;
    [SerializeField] private float linecastBuffer;
    private Vector3 ropePosA;
    private Vector3 ropePosB;
    private Vector3 ropePosC;
    private Vector3 ropePos1;
    private Vector3 ropePos2;
    private Vector3 ropePos3;
    [SerializeField] private LayerMask spikesLayerMask;
    [SerializeField] private LayerMask interactableLayerMask;
    //[SerializeField] private LayerMask wireLayerMask;
    private bool wireHasBeenDamaged;
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private bool snapping;

    public List<Vector3> ropePositions { get; set; } = new List<Vector3>();

    private void Awake()
    {
        Application.targetFrameRate = -1;
        int playerLayer = UnityEngine.LayerMask.NameToLayer("Player");
        int wireLayer = UnityEngine.LayerMask.NameToLayer("Wire");

        //Physics.IgnoreLayerCollision(playerLayer, wireLayer);
        player = GameObject.Find("Player Wire Anchor").transform;
        pm = player.GetComponentInParent<PlayerMovement>();
        AddPosToRope(startingPos.position);
    }
    private void Update()
    {
        UpdateRopePositions();


        if(wireHasBeenDamaged) return;
        LastSegmentGoToPlayerPos();
        DetectCollisionEnter();
        if(ropePositions.Count > 2) DetectCollisionExits();
        UpdateEdgeCollider(rope);
    }

    private void FixedUpdate()
    {
        UpdateRopePositions();

        if(wireHasBeenDamaged) return;
        DetectCollisionEnter();
        if(ropePositions.Count > 2) DetectCollisionExits();
    }

    private void DetectCollisionEnter()
    {
        RaycastHit2D hit = Physics2D.Linecast(player.position, rope.GetPosition(ropePositions.Count - 2), collMask);
        if(hit.collider != null)
        {
            //hit.transform.position = new Vector3(21.89f, 5.99f, 0);
            //Debug.Log($"hit: {hit.collider}, {hit.point}, {rope.GetPosition(ropePositions.Count - 2)}, {collMask}");
            if(Math.Abs(Vector2.Distance(rope.GetPosition(ropePositions.Count - 2), hit.point)) > minCollisionDistance)
            {
                if(hit.collider.gameObject.layer == 7)
                {
                    WireDamaged(hit.point);
                    return;
                }
                ropePositions.RemoveAt(ropePositions.Count - 1);
                AddPosToRope(hit.point, hit.collider.gameObject.CompareTag("Unsnappable"));
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        //if(collider.gameObject.layer != spikesLayerMask && collider.gameObject.layer != interactableLayerMask) return;
        //if(collider.gameObject.CompareTag("Player")) return;
        //Debug.Log(collider.gameObject.layer.ToString());
        //if(collider.gameObject.layer == 7) WireDamaged();
        if(collider.gameObject.layer == interactableLayerMask) Interact();
    }


    private void Interact()
    {
        Debug.Log("Interact");
    }

    private void WireDamaged(Vector3 _pos)
    {
        wireHasBeenDamaged = true;
        ropePositions.RemoveAt(ropePositions.Count - 1);
        //ropePositions.Add(_pos);

        GameObject newRope1 = new GameObject("RopePart1");
        LineRenderer newRope1LineRenderer = newRope1.AddComponent<LineRenderer>();
        newRope1.AddComponent<Rigidbody2D>();
        newRope1.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
       
        List<Vector3> newRope1Positions = new List<Vector3>();
        newRope1Positions.Add(_pos);
        newRope1Positions.Add(player.position);

        newRope1LineRenderer.positionCount = 2;
        newRope1LineRenderer.SetPosition(0, newRope1Positions[0]);
        newRope1LineRenderer.SetPosition(1, newRope1Positions[1]);
        newRope1LineRenderer.material = defaultMaterial;
        newRope1LineRenderer.SetWidth(0.1f, 0.1f);
        newRope1LineRenderer.SetColors(Color.black, Color.black);

        GameObject newRope2 = new GameObject("RopePart2");
        LineRenderer newRope2LineRenderer = newRope2.AddComponent<LineRenderer>();
        newRope2.AddComponent<Rigidbody2D>();
        newRope2.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

        List<Vector3> newRope2Positions = new List<Vector3>();
        newRope2Positions.Add(_pos);
        newRope2Positions.Add(ropePositions[ropePositions.Count - 1]);

        newRope2LineRenderer.positionCount = 2;
        newRope2LineRenderer.SetPosition(0, newRope2Positions[0]);
        newRope2LineRenderer.SetPosition(1, newRope2Positions[1]);
        newRope2LineRenderer.material = defaultMaterial;
        newRope2LineRenderer.SetWidth(0.1f, 0.1f);
        newRope2LineRenderer.SetColors(Color.black, Color.black);

        pm.OnDeath();

        Debug.Log($"distances: 1={Vector3.Distance(newRope1Positions[0], newRope1Positions[1])}, 2={Vector3.Distance(newRope2Positions[0], newRope2Positions[1])}");
        Debug.Log($"1 = {newRope1Positions[1]}, 2 = {newRope2Positions[1]}");
    }

    private void UpdateEdgeCollider(LineRenderer lineRenderer)
    {
        List<Vector2> edges = new List<Vector2>();

        for(int point = 0; point < lineRenderer.positionCount; point++)
        {
            Vector3 lineRendererPoint = lineRenderer.GetPosition(point);
            edges.Add(new Vector2(lineRendererPoint.x, lineRendererPoint.y));
        }

        edgeCollider.SetPoints(edges);
    }

    private void DetectCollisionExits()
    {
        ropePosA = player.position;
        ropePosB = rope.GetPosition(ropePositions.Count - 2);
        ropePosC = rope.GetPosition(ropePositions.Count - 3);

        ropePos1 = ropePosC + (ropePosA - ropePosC).normalized * (Vector3.Distance(ropePosC, ropePosA) * 0.25f);
        ropePos2 = ropePosC + (ropePosA - ropePosC).normalized * (Vector3.Distance(ropePosC, ropePosA) * 0.5f);
        ropePos3 = ropePosC + (ropePosA - ropePosC).normalized * (Vector3.Distance(ropePosC, ropePosA) * 0.75f);

        RaycastHit2D hit = Physics2D.Linecast(ropePosA, ropePosC + (ropePosA - ropePosC).normalized * linecastBuffer, collMask);
        //RaycastHit2D hit1 = Physics2D.Linecast(ropePosB + ((ropePosA - ropePosB) + (ropePosC - ropePosB) / 2).normalized * linecastBuffer, ropePos1, collMask);
        //RaycastHit2D hit2 = Physics2D.Linecast(ropePosB + ((ropePosA - ropePosB) + (ropePosC - ropePosB) / 2).normalized * linecastBuffer, ropePos2, collMask);
        //RaycastHit2D hit3 = Physics2D.Linecast(ropePosB + ((ropePosA - ropePosB) + (ropePosC - ropePosB) / 2).normalized * linecastBuffer, ropePos3, collMask);

        RaycastHit2D hit1 = Physics2D.Linecast(ropePosB + (ropePos1 - ropePosB).normalized * (linecastBuffer + Vector2.Distance(ropePosB, ropePos1) * .05f), ropePos1, collMask);
        RaycastHit2D hit2 = Physics2D.Linecast(ropePosB + (ropePos2 - ropePosB).normalized * (linecastBuffer + Vector2.Distance(ropePosB, ropePos2) * .05f), ropePos2, collMask);
        RaycastHit2D hit3 = Physics2D.Linecast(ropePosB + (ropePos3 - ropePosB).normalized * (linecastBuffer + Vector2.Distance(ropePosB, ropePos3) * .05f), ropePos3, collMask);


        Debug.Log($"{hit.collider == null}, {hit1.collider == null}, {hit2.collider == null}, {hit3.collider == null}");
        if(hit.collider == null && hit1.collider == null && hit2.collider == null && hit3.collider == null)
        {
            //if(hit.point.magnitude == 0) return;
            ropePositions.RemoveAt(ropePositions.Count - 2);
            return;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(ropePositions.Count > 2)
        {
            Gizmos.DrawLine(ropePosA, ropePosB);
            Gizmos.DrawLine(ropePosB, ropePosC);
            Gizmos.DrawLine(ropePosC, ropePosA);
            //Gizmos.DrawLine(ropePosB, ropePos1);
            //Gizmos.DrawLine(ropePosB, ropePos2);
            //Gizmos.DrawLine(ropePosB, ropePos3);
            //Gizmos.DrawLine(ropePosB + ((ropePosA - ropePosB) + (ropePosC - ropePosB) / 2).normalized * linecastBuffer, ropePos1);
            //Gizmos.DrawLine(ropePosB + ((ropePosA - ropePosB) + (ropePosC - ropePosB) / 2).normalized * linecastBuffer, ropePos2);
            //Gizmos.DrawLine(ropePosB + ((ropePosA - ropePosB) + (ropePosC - ropePosB) / 2).normalized * linecastBuffer, ropePos3);

            Gizmos.DrawLine(ropePosB + (ropePos1 - ropePosB).normalized * (linecastBuffer + Vector2.Distance(ropePosB, ropePos1) * .05f), ropePos1);
            Gizmos.DrawLine(ropePosB + (ropePos2 - ropePosB).normalized * (linecastBuffer + Vector2.Distance(ropePosB, ropePos2) * .05f), ropePos2);
            Gizmos.DrawLine(ropePosB + (ropePos3 - ropePosB).normalized * (linecastBuffer + Vector2.Distance(ropePosB, ropePos3) * .05f), ropePos3);
        }
    }

    private void AddPosToRope(Vector3 _pos, bool unsnappable = true)
    {
        if(snapping && !unsnappable) _pos = Vector3Int.RoundToInt(_pos);
        ropePositions.Add(_pos);
        ropePositions.Add(player.position); //Always the last pos must be the player
    }

    private void UpdateRopePositions()
    {
        rope.positionCount = ropePositions.Count;
        rope.SetPositions(ropePositions.ToArray());
    }

    private void LastSegmentGoToPlayerPos() => rope.SetPosition(rope.positionCount - 1, player.position);
}