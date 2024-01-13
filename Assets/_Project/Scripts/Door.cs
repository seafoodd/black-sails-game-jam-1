using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField] private int activated;
    [SerializeField] private int activationsNeeded;
    [SerializeField] private Vector3 defaultClosedPos;
    [SerializeField] private Vector3 defaultOpenPos;
    [SerializeField] private float moveDistance;
    [SerializeField] private Transform doorBottom;
    [SerializeField] private GameObject activationLight;
    [SerializeField] private SpriteRenderer doorTop;
    [SerializeField] private Sprite doorTopOn;
    [SerializeField] private Sprite doorTopOff;
    [SerializeField] private AudioSource aud;
    [SerializeField] private AudioClip openingSound;

    private void Awake()
    {
        if(activated > 0)
        {
            defaultOpenPos = doorBottom.position;
            defaultClosedPos = defaultOpenPos - new Vector3(0f, moveDistance);
        }
        else
        {
            defaultClosedPos = doorBottom.position;
            defaultOpenPos = defaultClosedPos + new Vector3(0f, moveDistance); 
            //GetComponent<SpriteRenderer>().color = Color.red;
            doorBottom.DOMove(defaultClosedPos, 1f);
        }
    }

    private void OnActivate()
    {
        //if(activated < activationsNeeded) return;
        activated++;
        if(activated >= activationsNeeded/* && Mathf.Abs(doorBottom.position.magnitude - defaultClosedPos.magnitude) < .1f*/)
        {
            //GetComponent<SpriteRenderer>().color = Color.green;
            doorTop.sprite = doorTopOn;
            activationLight.SetActive(true);
            //doorBottom.DOShakePosition(1, .1f, 100);
            doorBottom.DOMove(defaultOpenPos, 1f);
            aud.PlayOneShot(openingSound, .15f);

        }
    }

    private void OnDeactivate()
    {
        //if(activated >= activationsNeeded) return;
        activated--;
        if(activated < activationsNeeded)
        {
            //GetComponent<SpriteRenderer>().color = Color.red;
            doorTop.sprite = doorTopOff;
            activationLight.SetActive(false);
            doorBottom.DOMove(defaultClosedPos, 1f);
        }
    }
}
