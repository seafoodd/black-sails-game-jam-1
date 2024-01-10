using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField] private bool activated;
    [SerializeField] private Vector2 defaultClosedPos;
    [SerializeField] private Vector2 defaultOpenPos;
    [SerializeField] private float moveDistance;

    private void Awake()
    {
        if(activated)
        {
            defaultOpenPos = transform.position;
            defaultClosedPos = defaultOpenPos - new Vector2(0f, moveDistance);
        }
        else
        {
            defaultClosedPos = transform.position;
            defaultOpenPos = defaultClosedPos + new Vector2(0f, moveDistance); 
        }
    }

    private void OnActivate()
    {
        if(activated) return;
        activated = true;
        GetComponent<SpriteRenderer>().color = Color.green;
        transform.DOShakePosition(1, .1f, 100);
        transform.DOMove(defaultOpenPos, 1f);
    }

    private void OnDeactivate()
    {
        if(!activated) return;
        activated = false;
        GetComponent<SpriteRenderer>().color = Color.red;
        transform.DOMove(defaultClosedPos, 1f);
    }
}
