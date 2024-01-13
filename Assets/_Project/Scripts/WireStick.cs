using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireStick : MonoBehaviour
{

    private Vector2 anchorPos;
    [SerializeField] private Transform anchor;
    [SerializeField] private WireController wc;
    [SerializeField] private bool holdingWire;
    [SerializeField] private SpriteRenderer wireStick;
    [SerializeField] private SpriteRenderer wireStickTopLayer;
    [SerializeField] private Sprite wireStickHolding;
    [SerializeField] private Sprite wireStickHoldingTopLayer;

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

        wireStick.sprite = wireStickHolding;
        wireStickTopLayer.sprite = wireStickHoldingTopLayer;
        //Debug.Log("wire stick");
    }
}
