using Sourav.Utilities.Scripts.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sourav.Utilities.Scripts.Algorithms.AStarPathfinding
{
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
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, Speed * Time.deltaTime);
                yield return null;
            }
        }

        public void OnDrawGizmos()
        {
            if(path != null)
            {
                for (int i = targetIndex; i < path.Length; i++)
                {
                    //Gizmos.color = Color.black;
                    //Gizmos.DrawCube(path[i], Vector3.one);

                    if(i == targetIndex)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(transform.position, path[i]);
                    }
                    else
                    {
                        Gizmos.DrawLine(path[i - 1], path[i]);
                    }
                }
            }
        }
    }
}
