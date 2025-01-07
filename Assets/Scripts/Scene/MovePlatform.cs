using System;
using System.Collections.Generic;
using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [SerializeField] private List<MovementSpline> moveTransform;  // List of points for the platform to move between
    [SerializeField] private List<Vector3> movePoints;
    [SerializeField] private float moveSpeed = 2f;  // Speed of platform movement
    private int currentPointIndex = 0;  // Track the current point the platform is moving toward
    private int nextPointIndex = 1;
    private bool movingForward = true;  // Direction flag
    private float interpolateAmount;

    private void Start()
    {
        
        foreach (MovementSpline point in moveTransform)
        {
            movePoints.Add(point.point.position);
        }
        
        //moveTransform.Clear();
    }

    private void DetermineTargetPoint()
    {
        if (movingForward)
        {
            if (nextPointIndex + 1 >= moveTransform.Count)
            {
                nextPointIndex = currentPointIndex - 1;
                movingForward = false;
            }
            else
            {
                currentPointIndex++;
                nextPointIndex++;
            }
        }
        else
        {
            if (nextPointIndex - 1 < 0)
            {
                nextPointIndex = 1;
                movingForward = true;
            }
            else
            {
                currentPointIndex--;
                nextPointIndex--;
            }
        }
        Debug.Log(currentPointIndex);
        Debug.Log(nextPointIndex);
    }


    private void Update()
    {
        interpolateAmount = (interpolateAmount + Time.deltaTime) % 1f;
        if (Vector3.Distance(transform.position, movePoints[nextPointIndex]) < 0.1f)
        {
            DetermineTargetPoint();
        }
        this.transform.position = Vector3.Lerp(movePoints[currentPointIndex], movePoints[nextPointIndex], interpolateAmount);
    }
}

[Serializable]
public class MovementSpline
{
    public Transform point;
    public Transform midPoint;
}
