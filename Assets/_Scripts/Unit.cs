using Sourav.Utilities.Scripts.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform Target;

    public float Speed = 5.0f;
    private Vector3[] path;
    private int targetIndex;
    private Coroutine FollowPathCoroutine;

    void Start()
    {
        FindPath();
    }

    [Button()]
    public void FindPath()
    {
        PathRequestManager.Instance.RequestPath(transform.position, Target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if(pathSuccessful)
        {
            path = newPath;
            if(FollowPathCoroutine != null)
            {
                StopCoroutine(FollowPathCoroutine);
            }
            FollowPathCoroutine = StartCoroutine(FollowPath());
        }
    }

    private IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while(true)
        {
            if(transform.position == currentWaypoint)
            {
                targetIndex++;
                if(targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, Speed);
            yield return null;
        }
    }
}
