using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FlyingEye : MonoBehaviour
{
    public float flightSpeed = 2f;
    public float waypointReachedDistance = 0.1f;
    public DetectionZone biteDetectionZone;
    public List<Transform> waypoints;

    Animator animator;
    Rigidbody2D rb;
    Damageable damageable;

    Transform nextWayPoint;
    int waypointNum = 0;


    public bool _hasTarget = false;


    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }


    private void Awake()
    {

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        damageable = GetComponent<Damageable>();
    }

    private void Start()
    {
        nextWayPoint = waypoints[waypointNum]; 
    }
    // Update is called once per frame
    void Update()
    {
        HasTarget = biteDetectionZone.dectectedColliders.Count > 0;
    }

    private void FixedUpdate()
    {
        if (damageable.IsAlive)
        {
            if (CanMove)
            {
                Flight();
            }
            else
            {
                rb.velocity = Vector3.zero;
            }
        }

    }

    private void Flight()
    {
        Vector2 directionToWaypoint = (nextWayPoint.position - transform.position).normalized;

        float distance = Vector2.Distance(nextWayPoint.position, transform.position);

        rb.velocity = directionToWaypoint * flightSpeed;

        if (distance < waypointReachedDistance)
        {
            waypointNum++;

            if (waypointNum >= waypoints.Count)
            {
                waypointNum = 0;
            }

            nextWayPoint = waypoints[waypointNum];
        }
    }
}
