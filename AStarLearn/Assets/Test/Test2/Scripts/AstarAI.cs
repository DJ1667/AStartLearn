using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

namespace Test2
{
    public class AstarAI : MonoBehaviour
    {
        public Transform targetPosition;

        private CharacterController controller;
        private Seeker seeker;
        private int currentWaypoint = 0;
        public Path path;
        public  float speed = 5f;
        public float nextWaypointDistance = 3f;
        public  bool isReachEndOfPath = false;
        
        // Start is called before the first frame update
        void Start()
        {
            seeker = GetComponent<Seeker>();
            controller = GetComponent<CharacterController>();

            seeker.StartPath(transform.position, targetPosition.position, OnPathComplete);
        }

        // Update is called once per frame
        void Update()
        {
            if(path == null)
                return;

            isReachEndOfPath = false;
            float distanceToWayPoint;
            
            while (true)
            {
                distanceToWayPoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
                if (distanceToWayPoint < nextWaypointDistance)
                {
                    if(currentWaypoint + 1 < path.vectorPath.Count)
                        currentWaypoint++;
                    else
                    {
                        isReachEndOfPath = true;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            
            var speedFactor = isReachEndOfPath? Mathf.Sqrt(distanceToWayPoint / nextWaypointDistance) : 1f;
            
            Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
            Vector3 velocity = dir * speed * speedFactor;
            
            controller.SimpleMove(velocity);
        }

        private void OnPathComplete(Path p)
        {
            Debug.Log("Yay, we got a path back. Did it have an error? " + p.error);

            if (!p.error)
            {
                this.path = p;
                currentWaypoint = 0;
            }
        }
    }
}