using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private bool activated;

    [SerializeField] private GameObject activationLight;
    [SerializeField] private SpriteRenderer activationIndicator;
    [SerializeField] private Color colorActive;
    [SerializeField] private Color colorInactive;
    private Vector2 defaultButtonPos = new Vector2(0, 0.15f);
    private Vector2 pressedButtonPos;
    [SerializeField] private Transform button;

    [SerializeField] private GameObject gameObjectToActivate;

    private void Awake()
    {
        if(!activated) defaultButtonPos = button.localPosition;
        pressedButtonPos = new Vector2(0, button.localPosition.y - 0.1f);
    }

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
        activationIndicator.color = colorActive;
        activationLight.SetActive(true);
        button.localPosition = pressedButtonPos;

        if(gameObjectToActivate != null) gameObjectToActivate.SendMessage("OnActivate");
    }

    private void OnInactive()
    {
        if(!activated) return;
        activated = false;
        activationIndicator.color = colorInactive;
        activationLight.SetActive(false);
        button.localPosition = defaultButtonPos;

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
