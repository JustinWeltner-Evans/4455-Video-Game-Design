﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCS.Characters
{
    public class ProtagRootMotionHook : MonoBehaviour
    {
        private Animator anim;
        private Rigidbody rb;
        private Vector3 groundNormal;
        private Vector3 wallNormal;

        private Vector3 velocity;
        private bool grounded;
        private bool climbing;
        private bool aerial;

        private void Start()
        {
            anim = GetComponent<Animator>();
            rb = GetComponentInParent<Rigidbody>();
            groundNormal = GetComponentInParent<Protag>().getGroundNormal();
            wallNormal = Vector3.zero;
        }

        private void OnAnimatorMove()
        {
            Protag p = GetComponentInParent<Protag>();
            groundNormal = p.getGroundNormal();
            wallNormal = p.getClimableWallNormal();
            climbing = p.getClimbing();
            grounded = p.getGrounded();
            aerial = p.getAerial();

            if (anim.applyRootMotion)
            {
                Vector3 v = new Vector3(anim.deltaPosition.x, anim.deltaPosition.y, anim.deltaPosition.z) / Time.deltaTime;
                Vector3 dir = v.normalized;

                if (grounded && !aerial)
                {
                    dir = Vector3.ProjectOnPlane(v, groundNormal).normalized;
                    velocity = Vector3.ProjectOnPlane(v, groundNormal).magnitude * dir;
                    velocity = new Vector3(velocity.x, Mathf.Clamp(velocity.y, -20, 0), velocity.z);
                }
                else if (climbing)
                {
                    dir = Vector3.ProjectOnPlane(v, wallNormal).normalized;
                    velocity = v.magnitude * dir;
                }
                else if (aerial)
                {
                    velocity = new Vector3(velocity.x, v.y, velocity.z);
                } 
            }
        }

        private void FixedUpdate()
        {
            if (anim.applyRootMotion)
            {
                rb.velocity = velocity;
            }
        }
    }
}