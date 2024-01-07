using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float smoothing;
    
    [SerializeField] private Vector2 maxPosition;
    [SerializeField] private Vector2 minPosition;
    [SerializeField] private Transform topLeftMapCorner;
    [SerializeField] private Transform bottomRightMapCorner;

    private void Start()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;
        
        target = GameObject.Find("Player").transform;
        topLeftMapCorner = GameObject.Find("Top Left Map Corner").transform;
        bottomRightMapCorner = GameObject.Find("Bottom Right Map Corner").transform;
        
        Debug.Log($"{topLeftMapCorner.position}, {bottomRightMapCorner.position}");
        
        minPosition.x = topLeftMapCorner.position.x + width / 2;
        minPosition.y = bottomRightMapCorner.position.y + height / 2;
        
        maxPosition.x = bottomRightMapCorner.position.x - width / 2;
        maxPosition.y = topLeftMapCorner.position.y - height / 2;
    }

    void LateUpdate()
    {

        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

        targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition.x, maxPosition.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);
    }
}
