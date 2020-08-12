//-----------------------------------------------------------------------
// <copyright file="Projectile.cs" company="Google LLC">
//
// Copyright 2020 Google LLC. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

/// <summary>
/// A projected 3D cursor to guide the user in augmented reality.
/// </summary>
public class ProjectileCus : MonoBehaviour
{
    private Rigidbody m_Rigidbody;
    public bool isSetted {private set; get;}

    // Start is called before the first frame update
    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Rigidbody.velocity.Set(0, -0.2f, 0);
        Invoke("noGroundedDestroy", 5f);    //5秒內要著陸，不然要回收，代表沒碰到地面
        isSetted = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_Rigidbody.velocity.y >= -0.1f && !m_Rigidbody.isKinematic)
        {
            m_Rigidbody.isKinematic = true;
            m_Rigidbody.useGravity = false;
            GetComponent<Collider>().isTrigger = true;
            CancelInvoke("noGroundedDestroy");
            CreateAnchor();
            isSetted = true;
        }
    }

    private void noGroundedDestroy()
    {
        Destroy(gameObject);
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag.Equals("ARCoreCollider"))
        {
            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.angularVelocity = Vector3.zero;
        }
    }*/

    private void CreateAnchor()
    {
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);

        // Raycasts against the location the object stopped.
        TrackableHit hit;
        TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
            TrackableHitFlags.FeaturePointWithSurfaceNormal;

        if (Frame.Raycast(screenPoint.x, screenPoint.y, raycastFilter, out hit))
        {
            // Uses hit pose and camera pose to check if hittest is from the
            // back of the plane, if it is, no need to create the anchor.
            if ((hit.Trackable is DetectedPlane) &&
                Vector3.Dot(Camera.main.transform.position - hit.Pose.position,
                    hit.Pose.rotation * Vector3.up) < 0)
            {
                Debug.Log("Hit at back of the current DetectedPlane");
            }
            else
            {
                // Creats an anchor to allow ARCore to track the hitpoint as understanding of
                // the physical world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);
                transform.SetParent(anchor.transform, true);
            }
        }
    }
}
