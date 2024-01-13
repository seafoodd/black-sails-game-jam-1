using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private bool activated;

    [SerializeField] private GameObject activationLight;

    [SerializeField] private GameObject gameObjectToActivate;
    [SerializeField] private SpriteRenderer buttonSprite;
    [SerializeField] private Sprite buttonOn;
    [SerializeField] private Sprite buttonOff;


    private void OnTriggerEnter2D(Collider2D other)
    {
        OnActive();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        OnActive();
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        OnInactive();
    }
    private void OnActive()
    {
        if(activated) return;
        activated = true;
        buttonSprite.sprite = buttonOn;
        activationLight.SetActive(true);

        if(gameObjectToActivate != null) gameObjectToActivate.SendMessage("OnActivate");
    }

    private void OnInactive()
    {
        if(!activated) return;
        activated = false;
        buttonSprite.sprite = buttonOff;
        activationLight.SetActive(false);

        if(gameObjectToActivate != null) gameObjectToActivate.SendMessage("OnDeactivate");
    }

    /*private void Update()
    {
        if (activated)
        {
            activationIndicator.color = colorActive;
            activationLight.SetActive(true);
        }
        else
        {
            activationIndicator.color = colorInactive;
            activationLight.SetActive(false);
        }
    }*/
}
