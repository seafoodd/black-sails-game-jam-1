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
    [SerializeField] private AudioClip shakingSound;
    private bool opening;

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
            if(Mathf.Abs(doorBottom.position.magnitude - defaultClosedPos.magnitude) < .005f)
            {
                opening = true;
                aud.PlayOneShot(shakingSound, .3f);
                doorBottom.DOShakePosition(.75f, .1f, 100);
                Invoke("OpenDoor", .75f);
            }
            else
            {
                OpenDoor();
            }
            //doorBottom.DOMove(defaultOpenPos, 1f);
            //aud.PlayOneShot(openingSound, .15f);

        }
    }

    private void OpenDoor()
    {
        //Invoke("StopOpening", 1f);
        opening = false;
        doorBottom.DOMove(defaultOpenPos, 1f);
        aud.PlayOneShot(openingSound, .15f);
    }

    private void StopOpening()
    {
        opening = false;
    }

    private void OnDeactivate()
    {
        //if(activated >= activationsNeeded) return;
        activated--;
        if(activated < activationsNeeded)
        {
            //GetComponent<SpriteRenderer>().color = Color.red;
            if(opening)
            {
                CancelInvoke("OpenDoor");
                doorTop.sprite = doorTopOff;
                activationLight.SetActive(false);
                doorBottom.DOMove(defaultClosedPos, 1f);
            }
            else
            {
                doorTop.sprite = doorTopOff;
                activationLight.SetActive(false);
                doorBottom.DOMove(defaultClosedPos, 1f);
            }
        }
    }

    /*private void OnDeactivateWhileClosing()
    {
        //if(activated >= activationsNeeded) return;
        if(activated < activationsNeeded)
        {
            doorTop.sprite = doorTopOff;
            activationLight.SetActive(false);
            doorBottom.DOMove(defaultClosedPos, 1f);
        }
    }*/
}
