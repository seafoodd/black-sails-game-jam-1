using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public Transform player;

    public LineRenderer rope;
    public LayerMask collMask;
    [SerializeField] private float minCollisionDistance;
    [SerializeField] private float linecastBuffer;

    public List<Vector3> ropePositions { get; set; } = new List<Vector3>();

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        AddPosToRope(new Vector3(-7.5f, 0.08f, 0));
    }
    private void LateUpdate()
    {
        UpdateRopePositions();
        LastSegmentGoToPlayerPos();

        DetectCollisionEnter();
        if(ropePositions.Count > 2) DetectCollisionExits();
    }

    private void DetectCollisionEnter()
    {
        /*RaycastHit hit;
        if(Physics.Linecast(player.position, rope.GetPosition(ropePositions.Count - 2), out hit, collMask))
        {
            ropePositions.RemoveAt(ropePositions.Count - 1);
            AddPosToRope(hit.point);
        }*/

        RaycastHit2D hit = Physics2D.Linecast(player.position, rope.GetPosition(ropePositions.Count - 2), collMask);
        if(hit != null)
        {
            //hit.transform.position = new Vector3(21.89f, 5.99f, 0);
            Debug.Log($"hit: {hit.collider}, {hit.point}, {rope.GetPosition(ropePositions.Count - 2)}, {collMask}");
            if(System.Math.Abs(Vector2.Distance(rope.GetPosition(ropePositions.Count - 2), hit.point)) > minCollisionDistance && hit.point.magnitude != 0)
            {
                ropePositions.RemoveAt(ropePositions.Count - 1);
                AddPosToRope(hit.point);
            }
            /*ropePositions.RemoveAt(ropePositions.Count - 1);
            AddPosToRope(hit.point);*/
        }
    }

    private void DetectCollisionExits()
    {
        /*RaycastHit hit;
        if(!Physics.Linecast(player.position, rope.GetPosition(ropePositions.Count - 3), out hit, collMask))
        {
            ropePositions.RemoveAt(ropePositions.Count - 2);
        }*/
        Vector3 ropePos = rope.GetPosition(ropePositions.Count - 3);
        RaycastHit2D hit = Physics2D.Linecast(player.position, ropePos + (player.position - ropePos).normalized * linecastBuffer, collMask);
        if(hit.collider == null)
        {
            //if(hit.point.magnitude == 0) return;
            ropePositions.RemoveAt(ropePositions.Count - 2);
            return;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if(ropePositions.Count > 2) Gizmos.DrawLine(player.position, rope.GetPosition(ropePositions.Count - 3));
        if(ropePositions.Count > 1) Gizmos.DrawLine(player.position, rope.GetPosition(ropePositions.Count - 2));
    }

    private void AddPosToRope(Vector3 _pos)
    {
        ropePositions.Add(_pos);
        ropePositions.Add(player.position); //Always the last pos must be the player
        Debug.Log($"Added rope pos: {_pos}, {player.position}");
    }

    private void UpdateRopePositions()
    {
        rope.positionCount = ropePositions.Count;
        rope.SetPositions(ropePositions.ToArray());
    }

    private void LastSegmentGoToPlayerPos() => rope.SetPosition(rope.positionCount - 1, player.position);
}