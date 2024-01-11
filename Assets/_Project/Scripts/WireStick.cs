using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireStick : MonoBehaviour
{

    private Vector2 anchorPos;
    [SerializeField] private Transform anchor;
    [SerializeField] private WireController wc;
    [SerializeField] private bool holdingWire;

    private void Awake()
    {
        wc = GameObject.Find("Wire").GetComponent<WireController>();
        anchorPos = anchor.position;
        //Debug.Log($"anchor pos: {anchorPos}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(holdingWire) return;

        wc.AddPosToRope(anchorPos, true, false);
        holdingWire = true;
        //Debug.Log("wire stick");
    }
}
