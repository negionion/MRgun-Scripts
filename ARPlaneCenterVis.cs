using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using GoogleARCore.Examples.Common;

public class ARPlaneCenterVis : MonoBehaviour
{
	public GameObject centerPrefab;
	public UnityEngine.UI.Text log;
	private List<DetectedPlane> m_NewPlanes = new List<DetectedPlane>();
	// Start is called before the first frame update
	void Start()
    {
		InvokeRepeating("checkPlane", 5f, 3f);
    }

    // Update is called once per frame
    void Update()
    {
		
	}

	void checkPlane()
	{
		log.text = "x = ";
		if (Session.Status != SessionStatus.Tracking)
		{
			return;
		}
		log.text = "c = ";
		// Iterate over planes found in this frame and instantiate corresponding GameObjects to
		// visualize them.
		Session.GetTrackables<DetectedPlane>(m_NewPlanes, TrackableQueryFilter.New);
		Debug.Log(m_NewPlanes.Count);
		for (int i = 0; i < m_NewPlanes.Count; i++)
		{
			// Instantiate a plane visualization prefab and set it to track the new plane. The
			// transform is set to the origin with an identity rotation since the mesh for our
			// prefab is updated in Unity World coordinates.
			GameObject planeObject =
				Instantiate(centerPrefab, Vector3.zero, Quaternion.identity, transform);
			log.text += m_NewPlanes[i].CenterPose.position;
			//planeObject.GetComponent<DetectedPlaneVisualizer>().Initialize(m_NewPlanes[i]);
		}
	}
}
