//-----------------------------------------------------------------------
// <copyright file="HelloARController.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
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

using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Controls the HelloAR example.
/// </summary>
public class ARCoreCtrl : MonoBehaviour
{
	/// <summary>
	/// The first-person camera being used to render the passthrough camera image (i.e. AR
	/// background).
	/// </summary>
	public Camera FirstPersonCamera;

	/// <summary>
	/// The rotation in degrees need to apply to prefab when it is placed.
	/// </summary>
	private const float k_PrefabRotation = 180.0f;

	/// <summary>
	/// True if the app is in the process of quitting due to an ARCore connection error,
	/// otherwise false.
	/// </summary>
	private bool m_IsQuitting = false;

	/// <summary>
	/// The Unity Awake() method.
	/// </summary>
	/// 

	public void Awake()
	{
		// Enable ARCore to target 60fps camera capture frame rate on supported devices.
		// Note, Application.targetFrameRate is ignored when QualitySettings.vSyncCount != 0.
		Application.targetFrameRate = 60;
		//InvokeRepeating("detectedCorner", 10f, 0.2f);
	}

	/// <summary>
	/// The Unity Update() method.
	/// </summary>
	public void Update()
	{
		_UpdateApplicationLifecycle();

		//detectedCorner();
		
	}

	/// 舊版生成怪物及場景碰撞，已棄用，射擊碰撞改到各種gun的腳本中，怪物生成改到EnemyGanerator
	/// ----------------------已棄用----------------------
	/*private void detectedCorner()
	{
		TrackableHit hitL, hitR;
		int ans = 0;    //-1 = L, 1 = R
						//log.text = "test";
		TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon |
			TrackableHitFlags.FeaturePointWithSurfaceNormal;
		
		//-----------------------------
		if (GunControl.mtData.gunSt != 3)
			return;


		if (Frame.Raycast(Screen.width * 0.25f, Screen.height * 0.5f, raycastFilter, out hitL) && Frame.Raycast(Screen.width * 0.75f, Screen.height * 0.5f, raycastFilter, out hitR))
		{
			// Use hitL pose and camera pose to check if hittest is from the
			// back of the plane, if it is, no need to create the anchor.				
			if ((hitL.Trackable is DetectedPlane) &&
				Vector3.Dot(FirstPersonCamera.transform.position - hitL.Pose.position,
					hitL.Pose.rotation * Vector3.up) < 0)
			{
				Debug.Log("Hit at back of the current DetectedPlane");
			}
			else
			{
				GameObject prefab = enemy;

				if (hitL.Trackable is DetectedPlane && hitR.Trackable is DetectedPlane)
				{
					DetectedPlane planeL = hitL.Trackable as DetectedPlane;
					DetectedPlane planeR = hitR.Trackable as DetectedPlane;


					if (Vector3.Distance(hitR.Pose.position, hitL.Pose.position) > 0f)
					{

						if (planeL.PlaneType == DetectedPlaneType.Vertical && planeR.PlaneType == DetectedPlaneType.HorizontalUpwardFacing)
						{
							if (Vector3.Distance(hitR.Pose.position, FirstPersonCamera.transform.position) > Vector3.Distance(hitL.Pose.position, FirstPersonCamera.transform.position))
							{
								ans = 1;
								prefab = enemy;
							}
						}
						else if (planeR.PlaneType == DetectedPlaneType.Vertical && planeL.PlaneType == DetectedPlaneType.HorizontalUpwardFacing)
						{
							if (Vector3.Distance(hitR.Pose.position, FirstPersonCamera.transform.position) < Vector3.Distance(hitL.Pose.position, FirstPersonCamera.transform.position))
							{
								ans = -1;
								prefab = enemy;
							}
						}
					}
					else
					{
						ans = 0;
						prefab = enemy;
					}
				}

				Pose target;
				if (ans > 0)
					target = hitR.Pose;
				else if (ans < 0)
					target = hitL.Pose;
				else
					return;

				var gameObject = Instantiate(prefab, target.position, target.rotation);

				// Compensate for the hitPose rotation facing away from the raycast (i.e.
				// camera).
				gameObject.transform.Rotate(0, k_PrefabRotation, 0, Space.Self);

				// Create an anchor to allow ARCore to track the hitpoint as understanding of
				// the physical world evolves.
				var anchor = hitL.Trackable.CreateAnchor(target);

				// Make game object a child of the anchor.
				gameObject.transform.parent = anchor.transform;
			}
		}

	}*/
	/// ----------------------已棄用-----------------------

	/// <summary>
	/// Check and update the application lifecycle.
	/// </summary>
	private void _UpdateApplicationLifecycle()
	{
		// Exit the app when the 'back' button is pressed.
		if (Input.GetKey(KeyCode.Escape))
		{
			_DoQuit();
		}

		// Only allow the screen to sleep when not tracking.
		if (Session.Status != SessionStatus.Tracking)
		{
			Screen.sleepTimeout = SleepTimeout.SystemSetting;
		}
		else
		{
			Screen.sleepTimeout = SleepTimeout.NeverSleep;
		}

		if (m_IsQuitting)
		{
			return;
		}

		// Quit if ARCore was unable to connect and give Unity some time for the toast to
		// appear.
		if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
		{
			_ShowAndroidToastMessage("Camera permission is needed to run this application.");
			m_IsQuitting = true;
			Invoke("_DoQuit", 0.5f);
		}
		else if (Session.Status.IsError())
		{
			_ShowAndroidToastMessage(
				"ARCore encountered a problem connecting.  Please start the app again.");
			m_IsQuitting = true;
			Invoke("_DoQuit", 0.5f);
		}
	}

	/// <summary>
	/// Actually quit the application.
	/// </summary>
	private void _DoQuit()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene("home");
	}

	/// <summary>
	/// Show an Android toast message.
	/// </summary>
	/// <param name="message">Message string to show in the toast.</param>
	private void _ShowAndroidToastMessage(string message)
	{
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject unityActivity =
			unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

		if (unityActivity != null)
		{
			AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
			unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
			{
				AndroidJavaObject toastObject =
					toastClass.CallStatic<AndroidJavaObject>(
						"makeText", unityActivity, message, 0);
				toastObject.Call("show");
			}));
		}
	}
}

