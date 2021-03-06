﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float dampTime = 0.2f;
    public float screenEdgeBuffer = 4f;
    public float minimumSize = 6.5f;
    [HideInInspector] public Transform[] targets =  new Transform[3];

    Camera mainCamera;
    float zoomSpeed;
    Vector3 moveVelocity;
    Vector3 desiredPosition;

    private void Awake()
    {
        mainCamera = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        Move();
        Zoom();
    }

    private void Move()
    {
        FindAveragePosition();
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref moveVelocity, dampTime);
    }

    private void FindAveragePosition()
    {
        Vector3 averagePosition = new Vector3();
        int numberOfTargets = 0;

        for (int i = 0; i < targets.Length; i++)
        {
            if (!targets[i].gameObject.activeSelf)
                continue;
            else
            {
                averagePosition += targets[i].gameObject.transform.position;
                numberOfTargets++;
            }
        }

        if (numberOfTargets > 0)
            averagePosition /= numberOfTargets;
        averagePosition.y = this.gameObject.transform.position.y;
        desiredPosition = averagePosition;
    }

    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, requiredSize, ref zoomSpeed, dampTime);
    }

    private float FindRequiredSize()
    {
        Vector3 desiredLocalPosition = transform.InverseTransformPoint(desiredPosition);
        float size = 0f;
        for (int i = 0; i < targets.Length; i++)
        {
            if (!targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPosition = transform.InverseTransformPoint(targets[i].position);
            Vector3 desiredPositionToTarget = targetLocalPosition - desiredLocalPosition;

            size = Mathf.Max(size, Mathf.Abs(desiredPositionToTarget.y));
            size = Mathf.Max(size, Mathf.Abs(desiredPositionToTarget.x) / mainCamera.aspect);
        }
        size += screenEdgeBuffer;
        size = Mathf.Max(size, minimumSize);

        return size;
    }

    public void SetStartPositionAndSize()
    {
        FindAveragePosition();
        transform.position = desiredPosition;
        mainCamera.orthographicSize = FindRequiredSize();
    }
}
